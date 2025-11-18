using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libplctag;
using libplctag.DataTypes.Simple;

namespace Ring.Services.PLC
{
    public class PlcTagReader
    {
        private readonly string _tagName;
        private readonly string _ip;
        private readonly string _path;
        private readonly PlcDataType _type;

        public PlcTagReader(string tagName, string ip, PlcDataType type, string path = "1,0")
        {
            _tagName = tagName;
            _ip = ip;
            _path = path;
            _type = type;
        }
        
        private int ExtractArrayIndex(string tagName)
        {
            try
            {
                int startIndex = tagName.IndexOf("[") + 1;
                int endIndex = tagName.IndexOf("]");
                if (startIndex > 0 && endIndex > startIndex)
                {
                    string indexStr = tagName.Substring(startIndex, endIndex - startIndex);
                    return int.Parse(indexStr);
                }
            }
            catch
            {
                // If parsing fails, return 0
            }
            return 0;
        }

        public string Read()
        {
            try
            {
                switch (_type)
                {
                    case PlcDataType.BOOL:
                        // For boolean arrays in Allen-Bradley PLCs, we MUST use DINT read + bit extraction
                        // TagBool does NOT work correctly for individual array elements like Alarm_Triggers[1]
                        // Test results show: Direct BOOL read always returns False, but DINT method works correctly
                        //
                        // HOW IT WORKS:
                        // In AB PLCs, boolean arrays are packed into DINTs (32 booleans per DINT)
                        // - Alarm_Triggers[0] to Alarm_Triggers[31] are stored in Alarm_Triggers[0] as a DINT
                        // - Each bit in the DINT represents one boolean (bit 0 = [0], bit 1 = [1], etc.)
                        // - To read Alarm_Triggers[1], we read Alarm_Triggers[0] as DINT and extract bit 1
                        //
                        // Example: If Alarm_Triggers[1] = True, then Alarm_Triggers[0] as DINT = 2 (binary: 10)
                        //          because bit 1 is set (2^1 = 2)
                        
                        // Check if this is an array element (e.g., "Alarm_Triggers[1]")
                        if (_tagName.Contains("[") && _tagName.Contains("]"))
                        {
                            // Use DINT method for boolean arrays
                            try
                            {
                                int arrayIndex = ExtractArrayIndex(_tagName);
                                string baseTagName = _tagName.Substring(0, _tagName.IndexOf("["));
                                
                                // Calculate which DINT contains this boolean (32 bools per DINT)
                                // For Alarm_Triggers[1]: dintIndex = 1/32 = 0, bitPosition = 1%32 = 1
                                int dintIndex = arrayIndex / 32;
                                int bitPosition = arrayIndex % 32;
                                
                                // Read the DINT that contains this boolean
                                // For Alarm_Triggers[1], we read Alarm_Triggers[0] as DINT
                                var dintTag = new TagDint()
                                {
                                    Name = $"{baseTagName}[{dintIndex}]",
                                    Gateway = _ip,
                                    Path = _path,
                                    PlcType = PlcType.ControlLogix,
                                    Protocol = Protocol.ab_eip
                                };
                                
                                int dintValue = dintTag.Read();
                                // Extract the specific bit: bit 0 = [0], bit 1 = [1], bit 2 = [2], etc.
                                bool boolValue = (dintValue & (1 << bitPosition)) != 0;
                                string result = boolValue ? "True" : "False";
                                Console.WriteLine($"[PlcTagReader] Reading '{_tagName}': Read DINT '{baseTagName}[{dintIndex}]' = {dintValue}, extracted bit {bitPosition} = {result}");
                                return result;
                            }
                            catch (Exception dintEx)
                            {
                                Console.WriteLine($"[PlcTagReader] ✗ DINT read failed for '{_tagName}': {dintEx.Message}");
                                throw;
                            }
                        }
                        else
                        {
                            // For non-array BOOL tags, use direct BOOL read
                            try
                            {
                                var tagBool = new TagBool()
                                {
                                    Name = _tagName,
                                    Gateway = _ip,
                                    Path = _path,
                                    PlcType = PlcType.ControlLogix,
                                    Protocol = Protocol.ab_eip
                                };
                                
                                bool boolValue = tagBool.Read();
                                string result = boolValue ? "True" : "False";
                                Console.WriteLine($"[PlcTagReader] ✓ Read BOOL tag '{_tagName}': {result}");
                                return result;
                            }
                            catch (Exception boolEx)
                            {
                                Console.WriteLine($"[PlcTagReader] ✗ Direct BOOL read failed for '{_tagName}': {boolEx.Message}");
                                throw;
                            }
                        }

                    case PlcDataType.DINT:
                        var tagDint = new TagDint()
                        {
                            Name = _tagName,
                            Gateway = _ip,
                            Path = _path,
                            PlcType = PlcType.ControlLogix,
                            Protocol = Protocol.ab_eip
                        };
                        return tagDint.Read().ToString();

                    case PlcDataType.REAL:
                        var tagReal = new TagReal()
                        {
                            Name = _tagName,
                            Gateway = _ip,
                            Path = _path,
                            PlcType = PlcType.ControlLogix,
                            Protocol = Protocol.ab_eip
                        };
                        float realValue = tagReal.Read();
                        return realValue.ToString("0.###");

                    default:
                        return "Unsupported type";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PlcTagReader] ✗ Read error from tag '{_tagName}': {ex.Message}");
                Console.WriteLine($"[PlcTagReader] Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"[PlcTagReader] Inner exception: {ex.InnerException.Message}");
                }
                return "Error";
            }
        }
    }
}

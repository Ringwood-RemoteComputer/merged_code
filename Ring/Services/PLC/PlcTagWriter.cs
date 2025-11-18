using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libplctag;
using libplctag.DataTypes.Simple;
using Ring.Services;

namespace Ring.Services.PLC
{
    public class PlcTagWriter
    {
        private readonly string _tagName;
        private readonly string _ip;
        private readonly string _path;
        private readonly PlcDataType _type;

        public PlcTagWriter(string tagName, string ip, PlcDataType type, string path = "1,0")
        {
            _tagName = tagName;
            _ip = ip;
            _path = path;
            _type = type;
        }

        public bool Write(string value)
        {
            try
            {
                switch (_type)
                {
                    case PlcDataType.BOOL:
                        var tagBool = new TagBool()
                        {
                            Name = _tagName,
                            Gateway = _ip,
                            Path = _path,
                            PlcType = PlcType.ControlLogix,
                            Protocol = Protocol.ab_eip
                        };

                        if (bool.TryParse(value, out bool boolVal))
                        {
                            tagBool.Write(boolVal);
                            return true;
                        }
                        Console.WriteLine($"Invalid BOOL value: '{value}'");
                        break;

                    case PlcDataType.DINT:
                        var tagDint = new TagDint()
                        {
                            Name = _tagName,
                            Gateway = _ip,
                            Path = _path,
                            PlcType = PlcType.ControlLogix,
                            Protocol = Protocol.ab_eip
                        };

                        if (int.TryParse(value, out int intVal))
                        {
                            tagDint.Write(intVal);
                            return true;
                        }
                        Console.WriteLine($"Invalid DINT value: '{value}'");
                        break;

                    case PlcDataType.REAL:
                        var tagReal = new TagReal()
                        {
                            Name = _tagName,
                            Gateway = _ip,
                            Path = _path,
                            PlcType = PlcType.ControlLogix,
                            Protocol = Protocol.ab_eip
                        };

                        if (float.TryParse(value, out float floatVal))
                        {
                            tagReal.Write(floatVal);
                            return true;
                        }
                        Console.WriteLine($"Invalid REAL value: '{value}'");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Write error to {_tagName}: {ex.Message}");
            }

            return false;
        }
    }
}

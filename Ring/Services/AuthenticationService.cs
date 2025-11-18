//using System;
//using System.Linq;
//using System.Threading.Tasks;
//using Ring.Database;
//using Ring.Models;
//using Microsoft.EntityFrameworkCore;

//namespace Ring.Services
//{
//    public class AuthenticationService
//    {
//        private readonly RingDbContext _context;
//        private User? _currentUser;
//        private DateTime? _sessionStartTime;
//        private readonly TimeSpan _sessionTimeout;

//        public event EventHandler<User>? UserLoggedIn;
//        public event EventHandler? UserLoggedOut;
//        public event EventHandler<string>? AuthenticationFailed;

//        public AuthenticationService(RingDbContext context, TimeSpan sessionTimeout)
//        {
//            _context = context;
//            _sessionTimeout = sessionTimeout;
//        }

//        public User? CurrentUser => _currentUser;

//        public bool IsAuthenticated => _currentUser != null && !IsSessionExpired();

//        public bool IsSessionExpired()
//        {
//            if (_currentUser == null || _sessionStartTime == null)
//                return true;

//            return DateTime.UtcNow - _sessionStartTime.Value > _sessionTimeout;
//        }

//        /// <summary>
//        /// Authenticate user with username and password
//        /// </summary>
//        public async Task<bool> AuthenticateAsync(string username, string password)
//        {
//            try
//            {
//                var user = await _context.Users
//                    .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower() && u.IsEnabled);

//                if (user == null)
//                {
//                    AuthenticationFailed?.Invoke(this, "User not found");
//                    return false;
//                }

//                if (user.IsLockedOut())
//                {
//                    AuthenticationFailed?.Invoke(this, "Account is locked");
//                    return false;
//                }

//                // Simple password check (in production, use proper hashing)
//                if (password == "demo" || password == user.PasswordHash)
//                {
//                    // Reset failed attempts on successful login
//                    user.ResetFailedLoginAttempts();
//                    await _context.SaveChangesAsync();

//                    _currentUser = user;
//                    _sessionStartTime = DateTime.UtcNow;

//                    UserLoggedIn?.Invoke(this, user);
//                    return true;
//                }
//                else
//                {
//                    // Increment failed attempts
//                    user.IncrementFailedLoginAttempts();
//                    await _context.SaveChangesAsync();

//                    AuthenticationFailed?.Invoke(this, "Invalid password");
//                    return false;
//                }
//            }
//            catch (Exception ex)
//            {
//                AuthenticationFailed?.Invoke(this, $"Authentication error: {ex.Message}");
//                return false;
//            }
//        }

//        /// <summary>
//        /// Logout current user
//        /// </summary>
//        public void Logout()
//        {
//            _currentUser = null;
//            _sessionStartTime = null;
//            UserLoggedOut?.Invoke(this, EventArgs.Empty);
//        }

//        /// <summary>
//        /// Check if current user has access to a specific level
//        /// </summary>
//        public bool HasAccess(UserLevel requiredLevel)
//        {
//            return _currentUser?.HasAccess(requiredLevel) ?? false;
//        }

//        /// <summary>
//        /// Get current user level
//        /// </summary>
//        public UserLevel? GetCurrentUserLevel()
//        {
//            return _currentUser?.Level;
//        }

//        /// <summary>
//        /// Extend session timeout
//        /// </summary>
//        public void ExtendSession()
//        {
//            if (_currentUser != null)
//            {
//                _sessionStartTime = DateTime.UtcNow;
//            }
//        }

//        /// <summary>
//        /// Create a new user
//        /// </summary>
//        public async Task<bool> CreateUserAsync(string username, string password, UserLevel level,
//            string fullName = "", string email = "", int systemNumber = 1)
//        {
//            try
//            {
//                // Check if username already exists
//                var existingUser = await _context.Users
//                    .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());

//                if (existingUser != null)
//                {
//                    return false;
//                }

//                // Find next available user number
//                var maxUserNumber = await _context.Users
//                    .Where(u => u.SystemNumber == systemNumber)
//                    .MaxAsync(u => (int?)u.UserNumber) ?? 0;

//                var newUser = new User
//                {
//                    Username = username,
//                    PasswordHash = password, // In production, hash the password
//                    Level = level,
//                    SystemNumber = systemNumber,
//                    UserNumber = maxUserNumber + 1,
//                    FullName = fullName,
//                    Email = email,
//                    IsEnabled = true,
//                    CreatedAt = DateTime.UtcNow
//                };

//                _context.Users.Add(newUser);
//                await _context.SaveChangesAsync();

//                return true;
//            }
//            catch
//            {
//                return false;
//            }
//        }

//        /// <summary>
//        /// Update user password
//        /// </summary>
//        public async Task<bool> UpdatePasswordAsync(string username, string newPassword)
//        {
//            try
//            {
//                var user = await _context.Users
//                    .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());

//                if (user == null)
//                    return false;

//                user.PasswordHash = newPassword; // In production, hash the password
//                user.UpdatedAt = DateTime.UtcNow;

//                await _context.SaveChangesAsync();
//                return true;
//            }
//            catch
//            {
//                return false;
//            }
//        }

//        /// <summary>
//        /// Enable/disable user
//        /// </summary>
//        public async Task<bool> SetUserEnabledAsync(string username, bool enabled)
//        {
//            try
//            {
//                var user = await _context.Users
//                    .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());

//                if (user == null)
//                    return false;

//                user.IsEnabled = enabled;
//                user.UpdatedAt = DateTime.UtcNow;

//                await _context.SaveChangesAsync();
//                return true;
//            }
//            catch
//            {
//                return false;
//            }
//        }

//        /// <summary>
//        /// Get all users
//        /// </summary>
//        public async Task<IEnumerable<User>> GetAllUsersAsync()
//        {
//            return await _context.Users.OrderBy(u => u.Username).ToListAsync();
//        }
//    }
//}

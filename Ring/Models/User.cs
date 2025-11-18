using System;

namespace Ring.Models
{
    /// <summary>
    /// User levels equivalent to legacy system (PasswordForm.cpp)
    /// </summary>
    public enum UserLevel
    {
        None = 0,
        Operator1 = 1,
        Operator2 = 2,
        Operator3 = 3,
        Operator4 = 4,
        Supervisor1 = 5,
        Supervisor2 = 6,
        Supervisor3 = 7,
        Supervisor4 = 8,
        Ringwood = 9
    }

    /// <summary>
    /// User entity for authentication and authorization
    /// </summary>
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public UserLevel Level { get; set; }

        public int SystemNumber { get; set; }

        public int UserNumber { get; set; } // Array position in legacy system

        public string? FullName { get; set; }

        public string? Email { get; set; }

        public bool IsEnabled { get; set; } = true;

        public bool IsLocked { get; set; } = false;

        public DateTime? LastLoginTime { get; set; }

        public DateTime? LockoutEndTime { get; set; }

        public int FailedLoginAttempts { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Checks if user has access to a specific level
        /// </summary>
        public bool HasAccess(UserLevel requiredLevel)
        {
            return Level >= requiredLevel;
        }

        /// <summary>
        /// Checks if user is locked out
        /// </summary>
        public bool IsLockedOut()
        {
            return IsLocked || (LockoutEndTime.HasValue && LockoutEndTime.Value > DateTime.UtcNow);
        }

        /// <summary>
        /// Increments failed login attempts
        /// </summary>
        public void IncrementFailedLoginAttempts()
        {
            FailedLoginAttempts++;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Resets failed login attempts
        /// </summary>
        public void ResetFailedLoginAttempts()
        {
            FailedLoginAttempts = 0;
            IsLocked = false;
            LockoutEndTime = null;
            LastLoginTime = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Locks the user account
        /// </summary>
        public void LockAccount(TimeSpan lockoutDuration)
        {
            IsLocked = true;
            LockoutEndTime = DateTime.UtcNow.Add(lockoutDuration);
            UpdatedAt = DateTime.UtcNow;
        }
    }
}

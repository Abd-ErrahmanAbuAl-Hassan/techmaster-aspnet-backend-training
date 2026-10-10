namespace TrainingCenter.Application.Helpers
{
    /// <summary>
    /// Centralized collection of standardized result messages and error messages.
    /// Ensures consistency across all services and controllers.
    /// </summary>
    public static class ResultMessages
    {
        // ==================== Success Messages ====================
        public static class Success
        {
            public const string OperationCompleted = "Operation completed successfully.";
            public const string ResourceCreated = "{0} created successfully.";
            public const string ResourceUpdated = "{0} updated successfully.";
            public const string ResourceDeleted = "{0} deleted successfully.";
            public const string ResourceRetrieved = "{0} retrieved successfully.";
            public const string DataFetched = "Data fetched successfully.";
            public const string LoginSuccess = "Login successful.";
            public const string LogoutSuccess = "Logout successful.";
            public const string RegistrationSuccess = "Registration completed successfully.";
            public const string PasswordChanged = "Password changed successfully.";
            public const string EnrollmentSuccess = "Enrollment successful.";
            public const string PaymentSuccess = "Payment processed successfully.";
        }

        // ==================== Validation Error Messages ====================
        public static class Validation
        {
            public const string RequiredField = "{0} is required.";
            public const string InvalidFormat = "{0} format is invalid.";
            public const string InvalidLength = "{0} must be between {1} and {2} characters.";
            public const string InvalidRange = "{0} must be between {1} and {2}.";
            public const string PositiveNumber = "{0} must be a positive number.";
            public const string NonNegativeNumber = "{0} must be a non-negative number.";
            public const string EmailInvalid = "Email address is invalid.";
            public const string PhoneInvalid = "Phone number is invalid.";
            public const string PasswordWeak = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.";
            public const string DateRangeInvalid = "Start date must be before end date.";
            public const string PaginationPageInvalid = "Page number must be a positive number.";
            public const string PaginationSizeInvalid = "Page size must be between 1 and 50.";
            public const string EnumInvalid = "{0} contains an invalid value.";
            public const string DuplicateValue = "{0} already exists.";
        }

        // ==================== Not Found Messages ====================
        public static class NotFound
        {
            public const string ResourceNotFound = "{0} not found.";
            public const string RecordNotFound = "No records found matching your criteria.";
            public const string StudentNotFound = "Student not found.";
            public const string InstructorNotFound = "Instructor not found.";
            public const string TrackNotFound = "Training track not found.";
            public const string EnrollmentNotFound = "Enrollment not found.";
            public const string PaymentNotFound = "Payment not found.";
            public const string UserNotFound = "User account not found.";
        }

        // ==================== Conflict/Business Logic Messages ====================
        public static class Conflict
        {
            public const string DuplicateEmail = "An account with this email address already exists.";
            public const string DuplicatePhoneNumber = "An account with this phone number already exists.";
            public const string AlreadyEnrolled = "Student is already enrolled in this track.";
            public const string AlreadyAssigned = "Instructor is already assigned to this track.";
            public const string InsufficientCapacity = "The track has reached its capacity and no more enrollments are available.";
            public const string InvalidTrackStatus = "The track is not available at this time.";
            public const string InvalidUserStatus = "The User is not active at this time.";
            public const string InvalidEnrollmentStatus = "The enrollment status does not allow this action.";
            public const string TrackCodeGenerationFailed = "Failed to generate a unique track code. Please try again.";
            public const string ConcurrencyConflict = "The record was modified by another user. Please refresh and try again.";
            public const string InsufficientFunds = "Insufficient payment amount for this enrollment.";
        }

        // ==================== Authorization Messages ====================
        public static class Unauthorized
        {
            public const string InvalidCredentials = "Invalid email or password.";
            public const string AccessDenied = "You do not have permission to perform this action.";
            public const string TokenExpired = "Your session has expired. Please log in again.";
            public const string TokenInvalid = "Invalid authentication token.";
            public const string AccountInactive = "Your account is inactive. Please contact support.";
            public const string AccountLocked = "Your account has been locked due to multiple failed login attempts.";
        }

        // ==================== Server Error Messages ====================
        public static class ServerError
        {
            public const string UnexpectedError = "An unexpected error occurred. Please contact support if the problem persists.";
            public const string DatabaseError = "A database error occurred. The operation could not be completed.";
            public const string ProcessingError = "An error occurred while processing your request.";
            public const string EmailSendFailed = "Failed to send email. Please try again later.";
            public const string PaymentProcessingError = "An error occurred while processing the payment.";
            public const string FileUploadError = "Failed to upload file. Please try again.";
            public const string ExternalServiceError = "An external service error occurred. Please try again later.";
        }
    }
}
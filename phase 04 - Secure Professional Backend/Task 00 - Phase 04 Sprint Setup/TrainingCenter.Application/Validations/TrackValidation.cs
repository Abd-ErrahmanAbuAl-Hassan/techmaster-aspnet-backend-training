using TrainingCenter.Application.DTOs.Track.Requests;

namespace TrainingCenter.Application.Validations
{
    public class TrackValidation
    {
        public static List<string> ValidateTrackSession(CreateTrackSessionRequest request)
        {
            var errors = new List<string>();
            if (string.IsNullOrEmpty(request.Title)) errors.Add("Title is required.");
            if (string.IsNullOrEmpty(request.Description)) errors.Add("Description is required.");
            if (string.IsNullOrEmpty(request.MeetingLink)) errors.Add("Meeting link is required.");
            if (request.SessionDate < DateTime.UtcNow) errors.Add("Session date not valid.");
            if (request.InstructorId.HasValue && request.InstructorId.Value < 1 ) errors.Add("Instructor ID must be a positive number.");
            if (!TryValidateUrl(request.MeetingLink, out var error)) errors.Add(error!);

            return errors;
        }
        public static bool TryValidateUrl(string? url, out string? error)
        {
            error = null;

            if (string.IsNullOrWhiteSpace(url))
            {
                error = "URL is required.";
                return false;
            }

            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                error = "URL is not a valid absolute URI.";
                return false;
            }

            if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
            {
                error = "URL must use HTTP or HTTPS.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(uri.Host))
            {
                error = "URL must contain a host.";
                return false;
            }

            return true;
        }
    }
}

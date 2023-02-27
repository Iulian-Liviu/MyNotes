using Newtonsoft.Json;

namespace MyNotes.API.Models.ErrorResponse;


public class ResultResponse<T> {
    public int StatusCode { get; set; }

    public string? Message {
        get {
            return StatusType switch {
                ErrorResponse.StatusType.Success => "Your request was completed successfully.",
                ErrorResponse.StatusType.Bad => "Your request was invalid or malformed.",
                _ => "The resource you requested was not found."
            };
        }
    }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? DetailedMessage { get; set; }

    [JsonIgnore]
    public StatusType? StatusType { get; set; } = ErrorResponse.StatusType.Bad;

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public T? Data { get; set; }
}

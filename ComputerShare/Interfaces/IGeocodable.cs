
namespace ComputerShare.Interfaces
{
    public interface IGeocodable
    {
        string Postcode { get; }
        double Latitude { get; }
        double Longitude { get; }
        bool PostcodeHasBeenGeocoded { get; }

        void SetLocationImageUrl(string imageUrl);
    }
}


namespace ComputerShare.Services.PostcodeIo
{
    public class Postcodeio_Postcode
    {
        public string postcode { get; set; }
        public double longitude  { get; set; }
        public double latitude { get; set; }
        public string country { get; set; }
        public string region { get; set; }
        public string parish { get; set; }
    }

    public class Postcodeio_Query<T>
    {
        public string query { get; set; }
        public T result { get; set; }
    }

    public class Postcodeio_Result<T>
    {
        public int status { get; set; }
        public T result { get; set; }
    }

    public class Postcodeio_BulkPostcodeLookup
    {
        public string[] postcodes { get; set; }
    }
}

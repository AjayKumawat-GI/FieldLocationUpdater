namespace FieldLocationUpdater.ModelDto
{
    public class NominatimLocationDetailsResponseDto
    {
        public long place_id { get; set; }
        public string licence { get; set; }
        public string osm_type { get; set; }
        public long osm_id { get; set; }
        public string lat { get; set; }
        public string lon { get; set; }
        public string @class { get; set; } // Use '@' prefix as "class" is a reserved keyword in C#
        public string type { get; set; }
        public int place_rank { get; set; }
        public double importance { get; set; }
        public string addresstype { get; set; }
        public string name { get; set; }
        public string display_name { get; set; }
        public AddressResponseDto address { get; set; }
        public List<string> boundingbox { get; set; }
    }

    public class AddressResponseDto
    {
        public string hamlet { get; set; }
        public string village { get; set; }
        public string city { get; set; }
        public string county { get; set; }
        public string state_district { get; set; }
        public string state { get; set; }
        public string ISO3166_2_lvl4 { get; set; }
        public string postcode { get; set; }
        public string country { get; set; }
        public string country_code { get; set; }
    }
}

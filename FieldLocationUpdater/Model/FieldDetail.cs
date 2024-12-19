using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FieldLocationUpdater.Model
{
    [Table("Farm_FarmerDataTagging")]
    public class FieldDetail
    {
        [Key]
        public int tr_id { get; set; }
        public string Farmer_mobile { get; set; }
        public string season_year { get; set; }
        public string season_name { get; set; }
        public string feild_no { get; set; }
        public string coordinates { get; set; }
        public string area_ac { get; set; }
        public string userid { get; set; }
        public string Review_status { get; set; }
        public string Review_comment { get; set; }
        public DateTime upload_date { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string old_acre { get; set; }
        public string? isExcluded { get; set; }
        public string? qc_userid { get; set; }
        public string? is_qc_done { get; set; }
        public DateTime? ActivityStart_TimeStamp { get; set; }
        public DateTime? ActivityTimeStamp { get; set; }
        public string polygon_status { get; set; }
        public string qc_polygon_status { get; set; }
        public string qc_status { get; set; }
        public string qc_comment { get; set; }
        public DateTime? qc_timestamp { get; set; }
        public string? state { get; set; }
        public string? district { get; set; }
        public string? taluka { get; set; }
        public string? village { get; set; }
    }
}

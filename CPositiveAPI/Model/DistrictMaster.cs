using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CPositiveAPI.Model
{
    public class DistrictMaster
    {
        [Key]
        public int districtid { get; set; }
        public string districtname { get; set; }
        public int stateid { get; set; }

    }
}

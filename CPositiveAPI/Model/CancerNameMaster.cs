using System.ComponentModel.DataAnnotations;

namespace CPositiveAPI.Model
{
    public class CancerNameMaster
    {
        [Key]
        public int CancerNameId {  get; set; }
        public string CancerName { get; set; }
    }
}

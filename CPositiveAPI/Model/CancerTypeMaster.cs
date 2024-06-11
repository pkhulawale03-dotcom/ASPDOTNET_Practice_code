using System.ComponentModel.DataAnnotations;

namespace CPositiveAPI.Model
{
    public class CancerTypeMaster
    {
        [Key]
        public int CancerTypeId { get; set; }
        public string CancerType { get; set; }
    }
}

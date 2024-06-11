using System.ComponentModel.DataAnnotations;

namespace CPositiveAPI.Model
{
    public class StageMaster
    {
        [Key]
        public int stageId { get; set; }
        public string stagename { get; set; }
    }
}

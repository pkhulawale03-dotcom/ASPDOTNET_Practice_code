using System.ComponentModel.DataAnnotations;

namespace CPositiveAPI.Model
{
    public class GradeMaster
    {
        [Key]
        public int GradeId { get; set; }
        public string GradeName {  get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CPositiveAPI.Model
{
    public class StateMaster
    {
        [Key]
        public int stateid { get; set; }
        public string statename { get; set; }        
        public int CountryId { get; set; }      
    }
}

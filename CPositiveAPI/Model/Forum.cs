using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CPositiveAPI.Model
{
    public class Topic
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public DateTime? CreatedAt { get; set; }

        public int UserId { get; set; }
        public Users? User { get; set; }

        public ICollection<Discussion>? Discussions { get; set; } // navigation property
    }


    public class Discussion
    {
        [Key]
        public int Id { get; set; }
        public string? Content { get; set; }
        public DateTime? CreatedAt { get; set; }

        public int TopicId { get; set; }
        [ForeignKey("TopicId")]
        [JsonIgnore]
        public Topic? Topic { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public Users? User { get; set; } // rename property to User instead of Users
    }

}

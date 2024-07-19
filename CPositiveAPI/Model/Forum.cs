using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CPositiveAPI.Model
{
    public class Topic
    {
        [Key]
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public Users? Users { get; set; }
        //[JsonIgnore]
        public List<Discussion>? Discussions { get; set; }
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
        public Users? Users { get; set; }
    }
}

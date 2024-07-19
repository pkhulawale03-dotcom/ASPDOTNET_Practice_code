using CPositiveAPI.Data;
using CPositiveAPI.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CPositiveAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForumController : ControllerBase
    {
        public readonly ApplicationDbContext Context;

        private IConfiguration _configuration;
        public ForumController(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            Context = dbContext;
            _configuration = configuration;
        }
        
        // Get all topics
        [HttpGet("topics")]
        public IActionResult GetTopics()
        {
            var topics = Context.Topics.Include(t => t.Users).Include(t => t.Discussions).ToList();
            return Ok(topics);
        }

        // Get a specific topic by ID
        [HttpGet("topics/{id}")]
        public IActionResult GetTopic(int id)
        {
            var topic = Context.Topics.Include(t => t.Users).Include(t => t.Discussions).FirstOrDefault(t => t.Id == id);
            if (topic == null)
            {
                return NotFound();
            }
            return Ok(topic);
        }

        // Post a new topic
        [HttpPost("topics")]
        public IActionResult PostTopic(Topic topic)
        {
            Context.Topics.Add(topic);
            Context.SaveChanges();
            return CreatedAtAction(nameof(GetTopic), new { id = topic.Id }, topic);
        }

        // Get a all discussions for topic by topic ID
        [HttpGet("discussions/{id}")]
        public IActionResult GetDiscussion(int id)
        {
            var discussion = Context.Discussions.Include(d => d.Users).Include(d => d.Topic).Where(d => d.TopicId == id);
            if (discussion == null)
            {
                return NotFound();
            }
            return Ok(discussion);
        }

        // Post a new discussion
        [HttpPost("discussions")]
        public IActionResult PostDiscussion(Discussion discussion)
        {
            Context.Discussions.Add(discussion);
            Context.SaveChanges();
            return CreatedAtAction(nameof(GetDiscussion), new { id = discussion.Id }, discussion);
        }
    }
}

using CPositiveAPI.Data;
using CPositiveAPI.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.IO;

namespace CPositiveAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForumController : ControllerBase
    {
        public readonly ApplicationDbContext Context;
        private readonly ILogger<ForumController> _logger;

        private IConfiguration _configuration;
        public ForumController(ApplicationDbContext dbContext, IConfiguration configuration, ILogger<ForumController> logger)
        {
            Context = dbContext;
            _configuration = configuration;
            _logger = logger;
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

        [HttpGet("GetProfileImageByUserId/{userId}")]
        public async Task<IActionResult> GetProfileImageByUserId(int userId)
        {
            var user = await Context.PersonalDetails
                                      .Where(u => u.UserId == userId)
                                      .Select(u => new { u.ImagePath })
                                      .FirstOrDefaultAsync();

            if (user == null || string.IsNullOrEmpty(user.ImagePath))
            {
                return NotFound("User or image not found.");
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot//" + user.ImagePath);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Image file not found.");
            }

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            var bytes = System.IO.File.ReadAllBytes(filePath);
            return File(bytes, contentType, Path.GetFileName(filePath));
        }

        [HttpGet("GetCommentCount/{topicId}")]
        public async Task<IActionResult> GetCommentCountAndUserIds(int topicId)
        {
            var comments = await Context.Discussions
                                         .Where(c => c.TopicId == topicId)
                                         .ToListAsync();

            if (comments == null || comments.Count == 0)
            {
                return NotFound("No comments found for the given topic ID.");
            }

            var userIds = comments.Select(c => c.UserId).Distinct().ToList();
            var commentCount = comments.Count;

            return Ok(new
            {
                CommentCount = commentCount,
                UserIds = userIds
            });
        }

        [HttpGet("SearchTopics")]
        public async Task<IActionResult> SearchTopics(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return BadRequest("Query string is null or empty.");
            }

            var topics = await (from t in Context.Topics
                                join u in Context.Users on t.UserId equals u.UserId
                                where t.Content.Contains(query)
                                select new
                                {
                                    t.Id,
                                    t.Content,
                                    UserId = t.UserId,
                                    Username = u.Username
                                }).ToListAsync();

            if (topics == null || topics.Count == 0)
            {
                return NotFound("No topics found matching the search query.");
            }

            return Ok(topics);
        }



        //[HttpGet("Searchcommenttopics")]
        //public async Task<IActionResult> GetUsernamesForTopic(int topicId)
        //{
        //    // Assuming 'Topics' is a DbSet in your DbContext
        //    var userNames = await (from t in Context.Topics
        //                           join u in Context.Users on t.UserId equals u.UserId
        //                           where t.Id == topicId
        //                           select u.Username)
        //                           .Distinct() // Ensure usernames are unique
        //                           .ToListAsync();

        //    if (userNames == null || userNames.Count == 0)
        //    {
        //        return NotFound("No usernames found for the given topic ID.");
        //    }

        //    return Ok(userNames);
        //}

    }
}

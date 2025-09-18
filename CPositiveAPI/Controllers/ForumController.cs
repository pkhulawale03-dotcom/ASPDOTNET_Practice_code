using CPositiveAPI.Interfaces;
using CPositiveAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace CPositiveAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForumController : ControllerBase
    {
        private readonly IForumRepository _forumRepository;

        public ForumController(IForumRepository forumRepository)
        {
            _forumRepository = forumRepository;
        }

        [HttpGet("topics")]
        public IActionResult GetTopics()
        {
            var topics = _forumRepository.GetTopics();
            return Ok(topics);
        }

        [HttpGet("topics/{id}")]
        public IActionResult GetTopic(int id)
        {
            var topic = _forumRepository.GetTopicById(id);
            if (topic == null) return NotFound();
            return Ok(topic);
        }

        [HttpPost("topics")]
        public IActionResult PostTopic(Topic topic)
        {
            _forumRepository.AddTopic(topic);
            _forumRepository.Save();
            return CreatedAtAction(nameof(GetTopic), new { id = topic.Id }, topic);
        }

        [HttpGet("discussions/{id}")]
        public IActionResult GetDiscussion(int id)
        {
            var discussions = _forumRepository.GetDiscussionsByTopicId(id);
            if (!discussions.Any()) return NotFound();
            return Ok(discussions);
        }

        [HttpPost("discussions")]
        public IActionResult PostDiscussion(Discussion discussion)
        {
            _forumRepository.AddDiscussion(discussion);
            _forumRepository.Save();
            return CreatedAtAction(nameof(GetDiscussion), new { id = discussion.Id }, discussion);
        }

        [HttpGet("GetProfileImageByUserId/{userId}")]
        public async Task<IActionResult> GetProfileImageByUserId(int userId)
        {
            var imagePath = await _forumRepository.GetProfileImageByUserId(userId);

            if (string.IsNullOrEmpty(imagePath))
                return NotFound("User or image not found.");

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot//" + imagePath);
            if (!System.IO.File.Exists(filePath))
                return NotFound("Image file not found.");

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
            var (count, userIds) = await _forumRepository.GetCommentCountAndUserIds(topicId);

            if (count == 0) return NotFound("No comments found for the given topic ID.");

            return Ok(new { CommentCount = count, UserIds = userIds });
        }

        [HttpGet("SearchTopics")]
        public async Task<IActionResult> SearchTopics(string query)
        {
            if (string.IsNullOrEmpty(query)) return BadRequest("Query string is null or empty.");

            var topics = await _forumRepository.SearchTopics(query);
            if (!topics.Any()) return NotFound("No topics found matching the search query.");

            return Ok(topics);
        }
    }
}

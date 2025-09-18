using CPositiveAPI.Data;
using CPositiveAPI.Interfaces;
using CPositiveAPI.Model;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CPositiveAPI.Repositories
{
    public class ForumRepository : IForumRepository
    {
        private readonly ApplicationDbContext _context;

        public ForumRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all topics
        public IEnumerable<Topic> GetTopics()
        {
            return _context.Topics.Include(t => t.Users).Include(t => t.Discussions).ToList();
        }

        // Get specific topic by ID
        public Topic GetTopicById(int id)
        {
            return _context.Topics
                .Include(t => t.Users)
                .Include(t => t.Discussions)
                .FirstOrDefault(t => t.Id == id);
        }

        // Add new topic
        public void AddTopic(Topic topic)
        {
            _context.Topics.Add(topic);
        }

        // Get discussions for topic
        public IEnumerable<Discussion> GetDiscussionsByTopicId(int topicId)
        {
            return _context.Discussions
                .Include(d => d.Users)
                .Include(d => d.Topic)
                .Where(d => d.TopicId == topicId)
                .ToList();
        }

        // Add discussion
        public void AddDiscussion(Discussion discussion)
        {
            _context.Discussions.Add(discussion);
        }

        // Get profile image path
        public async Task<string?> GetProfileImageByUserId(int userId)
        {
            var user = await _context.PersonalDetails
                .Where(u => u.UserId == userId)
                .Select(u => u.ImagePath)
                .FirstOrDefaultAsync();

            return user;
        }

        // Get comment count + userIds
        public async Task<(int commentCount, List<int> userIds)> GetCommentCountAndUserIds(int topicId)
        {
            var comments = await _context.Discussions
                .Where(c => c.TopicId == topicId)
                .ToListAsync();

            if (comments.Count == 0)
                return (0, new List<int>());

            var userIds = comments.Select(c => c.UserId).Distinct().ToList();
            return (comments.Count, userIds);
        }

        // Search topics
        public async Task<IEnumerable<object>> SearchTopics(string query)
        {
            return await (from t in _context.Topics
                          join u in _context.Users on t.UserId equals u.UserId
                          where t.Content.Contains(query)
                          select new
                          {
                              t.Id,
                              t.Content,
                              UserId = t.UserId,
                              Username = u.Username
                          }).ToListAsync();
        }

        // Save Changes
        public bool Save()
        {
            return _context.SaveChanges() > 0;
        }
    }
}

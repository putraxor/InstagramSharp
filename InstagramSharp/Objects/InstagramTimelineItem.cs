using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramSharp.Objects
{
    class InstagramTimelineItem
    {
        public class Candidate
        {
            public string url { get; set; }
            public int width { get; set; }
            public int height { get; set; }

            public Candidate(JToken JCandidate)
            {
                url = JCandidate["url"].ToString();
                width = int.Parse(JCandidate["width"].ToString());
                height = int.Parse(JCandidate["height"].ToString());
            }
        }

        public class ImageVersions2
        {
            List<Candidate> candidates { get; set; }

            public ImageVersions2(JToken JCandidates)
            {
                candidates = new List<Candidate>();
                foreach(JToken JCandidate in JCandidates["candidates"])
                {
                    Candidate candidate = new Candidate(JCandidate);
                    candidates.Add(candidate);
                }
            }
        }

        public class Comment
        {
            public string status { get; set; }
            public int user_id { get; set; }
            public int created_at_utc { get; set; }
            public int created_at { get; set; }
            public int bit_flags { get; set; }
            public InstagramUser user { get; set; }
            public string content_type { get; set; }
            public string text { get; set; }
            public long media_id { get; set; }
            public long pk { get; set; }
            public int type { get; set; }

            public Comment(JToken JComment)
            {
                status = JComment["status"].ToString();
                user_id = int.Parse(JComment["user_id"].ToString());
                created_at_utc = int.Parse(JComment["created_at_utc"].ToString());
                created_at = int.Parse(JComment["created_at"].ToString());
                bit_flags = int.Parse(JComment["bit_flags"].ToString());
                user = new InstagramUser(JComment["user"]);
                content_type = JComment["content_type"].ToString();
                text = JComment["text"].ToString();
                media_id = long.Parse(JComment["media_id"].ToString());
                pk = long.Parse(JComment["pk"].ToString());
                type = int.Parse(JComment["type"].ToString());
            }
        }

        public string code { get; set; }
        public int max_num_visible_preview_comments { get; set; }
        public int like_count { get; set; }
        public ImageVersions2 image_versions2 { get; set; }
        public string id { get; set; }
        public string client_cache_key { get; set; }
        public List<Comment> comments { get; set; }
        public double device_timestamp { get; set; }
        public int comment_count { get; set; }
        public int media_type { get; set; }
        public string organic_tracking_token { get; set; }
        public bool caption_is_edited { get; set; }
        public int original_height { get; set; }
        public int filter_type { get; set; }
        public InstagramUser user { get; set; }
        public long pk { get; set; }
        public bool has_liked { get; set; }
        public bool has_more_comments { get; set; }
        public bool photo_of_you { get; set; }
        public Comment caption { get; set; }
        public double taken_at { get; set; }
        public int original_width { get; set; }

        public InstagramTimelineItem(JToken JItem)
        {
            code = JItem["code"].ToString();
            max_num_visible_preview_comments = int.Parse(JItem["max_num_visible_preview_comments"].ToString());
            like_count = int.Parse(JItem["like_count"].ToString());
            image_versions2 = new ImageVersions2(JItem["image_versions2"]);
            id = JItem["id"].ToString();
            client_cache_key = JItem["client_cache_key"].ToString();
            comments = new List<Comment>();
            foreach (JToken JComment in JItem["comments"])
            {
                Comment comment = new Comment(JComment);
                comments.Add(comment);
            }
            device_timestamp = double.Parse(JItem["device_timestamp"].ToString());
            comment_count = int.Parse(JItem["comment_count"].ToString());
            media_type = int.Parse(JItem["media_type"].ToString());
            organic_tracking_token = JItem["organic_tracking_token"].ToString();
            caption_is_edited = bool.Parse(JItem["caption_is_edited"].ToString());
            original_height = int.Parse(JItem["original_height"].ToString());
            filter_type = int.Parse(JItem["filter_type"].ToString());
            user = new InstagramUser(JItem["user"]);
            pk = long.Parse(JItem["pk"].ToString());
            has_liked = bool.Parse(JItem["has_liked"].ToString());
            has_more_comments = bool.Parse(JItem["has_more_comments"].ToString());
            photo_of_you = bool.Parse(JItem["photo_of_you"].ToString());
            caption = new Comment(JItem["caption"]);
            taken_at = double.Parse(JItem["taken_at"].ToString());
            original_width = int.Parse(JItem["original_width"].ToString());
        }
    }
}

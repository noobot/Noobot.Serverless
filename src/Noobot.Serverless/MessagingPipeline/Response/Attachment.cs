using System.Collections.Generic;
using System.Linq;

namespace Noobot.Serverless.MessagingPipeline.Response
{
    public class Attachment
    {
        public Attachment(
            string text,
            string title,
            string authorName,
            string fallback,
            string imageUrl,
            string thumbUrl,
            string color,
            IEnumerable<AttachmentField> attachmentFields = null)
        {
            Text = text;
            Title = title;
            AuthorName = authorName;
            Fallback = fallback;
            ImageUrl = imageUrl;
            ThumbUrl = thumbUrl;
            Color = color;
            _attachmentFields = (attachmentFields ?? new List<AttachmentField>()).ToList();
        }

        public string Text { get; }
        public string Title { get; }
        public string AuthorName { get; }
        public string Fallback { get; }

        public string ImageUrl { get; }
        public string ThumbUrl { get; }

        public string Color { get; }

        public IEnumerable<AttachmentField> AttachmentFields => _attachmentFields;
        private readonly List<AttachmentField> _attachmentFields;

        public Attachment AddAttachmentField(string title, string value)
        {
            return AddAttachmentField(title, value, false);
        }

        public Attachment AddAttachmentField(string title, string value, bool isShort)
        {
            _attachmentFields.Add(new AttachmentField(title, value, isShort));
            return this;
        }
    }
}
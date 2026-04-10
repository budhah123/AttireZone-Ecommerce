using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AttireZone_Web_App.DataAccess;
using AttireZone_Web_App.Models;

namespace AttireZone_Web_App.BusinessLogic
{
    public class FeedbackService
    {
        public static bool AddFeedback(Feedback feedback)
        {
            if (!IsFeedbackValid(feedback))
            {
                return false;
            }

            EnsureFeedbackSchema();

            const string sql = @"
INSERT INTO [dbo].[Feedback]
(
    [UserId],
    [ProductId],
    [Rating],
    [Comment],
    [CreatedAt]
)
VALUES
(
    @UserId,
    @ProductId,
    @Rating,
    @Comment,
    GETDATE()
);";

            var normalizedComment = NormalizeComment(feedback.Comment);

            SqlParameter[] parameters =
            {
                new SqlParameter("@UserId", SqlDbType.Int) { Value = feedback.UserId },
                new SqlParameter("@ProductId", SqlDbType.Int) { Value = feedback.ProductId },
                new SqlParameter("@Rating", SqlDbType.Int) { Value = feedback.Rating },
                new SqlParameter("@Comment", SqlDbType.NVarChar, 1000)
                {
                    Value = string.IsNullOrWhiteSpace(normalizedComment) ? (object)DBNull.Value : normalizedComment
                }
            };

            try
            {
                return DBHelper.ExecuteNonQuery(sql, parameters) > 0;
            }
            catch
            {
                return false;
            }
        }

        public static List<Feedback> GetFeedbackByProductId(int productId, int take = 6)
        {
            if (productId <= 0)
            {
                return new List<Feedback>();
            }

            var safeTake = take <= 0 ? 6 : Math.Min(take, 50);

            const string sql = @"
SELECT TOP (@Take)
    f.[FeedbackId],
    f.[UserId],
    f.[ProductId],
    f.[Rating],
    f.[Comment],
    f.[CreatedAt],
    u.[FullName] AS [UserFullName]
FROM [dbo].[Feedback] f
LEFT JOIN [dbo].[Users] u ON u.[UserId] = f.[UserId]
WHERE f.[ProductId] = @ProductId
ORDER BY f.[CreatedAt] DESC, f.[FeedbackId] DESC;";

            SqlParameter[] parameters =
            {
                new SqlParameter("@Take", SqlDbType.Int) { Value = safeTake },
                new SqlParameter("@ProductId", SqlDbType.Int) { Value = productId }
            };

            try
            {
                EnsureFeedbackSchema();

                DataTable dt = DBHelper.ExecuteDataTable(sql, parameters);
                var feedbackEntries = new List<Feedback>(dt.Rows.Count);

                for (var index = 0; index < dt.Rows.Count; index++)
                {
                    feedbackEntries.Add(MapFeedback(dt.Rows[index]));
                }

                return feedbackEntries;
            }
            catch
            {
                return new List<Feedback>();
            }
        }

        private static void EnsureFeedbackSchema()
        {
            const string sql = @"
IF OBJECT_ID(N'dbo.Feedback', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Feedback]
    (
        [FeedbackId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [UserId] INT NOT NULL,
        [ProductId] INT NOT NULL,
        [Rating] INT NOT NULL,
        [Comment] NVARCHAR(1000) NULL,
        [CreatedAt] DATETIME NOT NULL CONSTRAINT [DF_Feedback_CreatedAt] DEFAULT GETDATE(),
        CONSTRAINT [CK_Feedback_Rating] CHECK ([Rating] BETWEEN 1 AND 5)
    );
END;

IF COL_LENGTH('dbo.Feedback', 'CreatedAt') IS NULL
BEGIN
    ALTER TABLE [dbo].[Feedback]
        ADD [CreatedAt] DATETIME NOT NULL CONSTRAINT [DF_Feedback_CreatedAt_Auto] DEFAULT GETDATE();
END;

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_Feedback_UserId'
      AND object_id = OBJECT_ID(N'dbo.Feedback')
)
BEGIN
    CREATE INDEX [IX_Feedback_UserId] ON [dbo].[Feedback]([UserId]);
END;

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_Feedback_ProductId'
      AND object_id = OBJECT_ID(N'dbo.Feedback')
)
BEGIN
    CREATE INDEX [IX_Feedback_ProductId] ON [dbo].[Feedback]([ProductId]);
END;";

            DBHelper.ExecuteNonQuery(sql);
        }

        private static bool IsFeedbackValid(Feedback feedback)
        {
            if (feedback == null)
            {
                return false;
            }

            if (feedback.UserId <= 0 || feedback.ProductId <= 0)
            {
                return false;
            }

            if (feedback.Rating < 1 || feedback.Rating > 5)
            {
                return false;
            }

            return !string.IsNullOrWhiteSpace(NormalizeComment(feedback.Comment));
        }

        private static Feedback MapFeedback(DataRow row)
        {
            return new Feedback
            {
                FeedbackId = Convert.ToInt32(row["FeedbackId"]),
                UserId = Convert.ToInt32(row["UserId"]),
                ProductId = Convert.ToInt32(row["ProductId"]),
                Rating = row["Rating"] == DBNull.Value ? 0 : Convert.ToInt32(row["Rating"]),
                Comment = row["Comment"] == DBNull.Value ? string.Empty : row["Comment"].ToString(),
                CreatedAt = row["CreatedAt"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["CreatedAt"]),
                UserFullName = row.Table.Columns.Contains("UserFullName") && row["UserFullName"] != DBNull.Value
                    ? row["UserFullName"].ToString()
                    : string.Empty
            };
        }

        private static string NormalizeComment(string comment)
        {
            if (string.IsNullOrWhiteSpace(comment))
            {
                return null;
            }

            var normalized = comment.Trim();
            if (normalized.Length > 1000)
            {
                normalized = normalized.Substring(0, 1000).TrimEnd();
            }

            return normalized;
        }
    }
}

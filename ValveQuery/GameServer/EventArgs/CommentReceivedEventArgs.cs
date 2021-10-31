namespace ValveQuery.GameServer
{
	/// <summary>
	/// Provides data for Comment Received event.
	/// </summary>
	public class CommentReceivedEventArgs : LogEventArgs
	{
		/// <summary>
		/// Comment.
		/// </summary>
		public string Comment { get; set; }
	}
}

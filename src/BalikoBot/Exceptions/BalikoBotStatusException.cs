using System.Net;

namespace BalikoBot
{
	/// <summary>
	/// vyjimka BalikoBotu z http statusu
	/// </summary>
	public class BalikoBotStatusException : BalikoBotException
	{
		/// <summary>
		/// status kod z API BalikoBotu
		/// </summary>
		public HttpStatusCode StatusCode { get; private set; }

		internal BalikoBotStatusException(HttpStatusCode statusCode)
		{
			StatusCode = statusCode;
		}
	}
}

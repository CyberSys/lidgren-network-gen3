using System;
using System.Collections.Generic;
using System.Text;

namespace Lidgren.Network
{
	/// <summary>
	/// How the library deals with resends and handling of late messages
    /// 怎样分配 重发和迟到消息处理
	/// </summary>
	public enum NetDeliveryMethod : byte
	{
		//
		// Actually a publicly visible subset of NetMessageType
		//

		/// <summary>
		/// Indicates an error
        /// 指示错误
		/// </summary>
		Unknown = 0,

		/// <summary>
		/// Unreliable, unordered delivery
        /// 不可靠的，无序的传递
		/// </summary>
		Unreliable = 1,

		/// <summary>
		/// Unreliable delivery, but automatically dropping late messages
        /// 不可靠的传递,自动丢弃迟到的消息
		/// </summary>
		UnreliableSequenced = 2,

		/// <summary>
		/// Reliable delivery, but unordered
        /// 可靠的传递，无序传送
		/// </summary>
		ReliableUnordered = 34,

		/// <summary>
		/// Reliable delivery, except for late messages which are dropped
        /// 可靠的传递，除了被丢弃的迟到的消息
		/// </summary>
		ReliableSequenced = 35,

		/// <summary>
		/// Reliable, ordered delivery
        /// 可靠，有序传送
		/// </summary>
		ReliableOrdered = 67,
	}
}

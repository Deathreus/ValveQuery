using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Sockets;

namespace ValveQuery.GameServer
{
	/// <summary>
	/// Represents the connected server.Provides methods to query,listen to server logs and control the server.
	/// </summary>
	public class Server : QueryBase
	{
		private long Latency;

		private Logs Logs;

		private byte[] PlayerChallengeId;
		private byte[] RuleChallengeId;
		private byte[] InfoChallengeId;

		private bool IsPlayerChallengeId;
		private bool IsRuleChallengeId;
		private bool IsInfoChallengeId;

		internal UdpQuery UdpSocket;

		internal ConnectionInfo ConInfo;

		/// <summary>
		/// Server Endpoint.
		/// </summary>
		public IPEndPoint EndPoint { get; protected set; }

		/// <summary>
		/// Provides method(s) to perform admin level operations.
		/// </summary>
		public Rcon Rcon { get; protected set; }

		/// <summary>
		/// Gets or sets Socket's SendTimeout Property.
		/// </summary>
		public int SendTimeout
		{
			get => UdpSocket.Socket.SendTimeout;
			set => UdpSocket.Socket.SendTimeout = value;
		}

		/// <summary>
		/// Gets or sets Socket's ReceiveTimeout.
		/// </summary>
		public int ReceiveTimeout
		{
			get => UdpSocket.Socket.ReceiveTimeout;
			set => UdpSocket.Socket.ReceiveTimeout = value;
		}

		internal Server(ConnectionInfo conInfo)
		{
			ConInfo = conInfo;
			EndPoint = conInfo.EndPoint;
			UdpSocket = new UdpQuery(conInfo);
		}

		/// <summary>
		/// Retrieves information about the server.
		/// </summary>
		/// <param name="callback">Called on every attempt made to connect to server(max. attempts = Retries + 1).</param>
		/// <returns>Instance of ServerInfo.</returns>
		public virtual ServerInfo GetInfo(AttemptCallback callback = null)
		{
			ThrowIfDisposed();
			return Invoke(InternalGetInfo, ConInfo.Retries + 1, callback, ConInfo.ThrowExceptions);
		}

		private ServerInfo InternalGetInfo()
		{
			byte[] recvData = null;
			try
			{
				if (InfoChallengeId == null)
				{
					recvData = GetInfoChallengeId();
					if (IsInfoChallengeId)
					{
						InfoChallengeId = recvData;
					}
				}

				var stopwatch = Stopwatch.StartNew();
				if (IsInfoChallengeId)
				{
					recvData = UdpSocket.GetResponse(Util.MergeByteArrays(QueryMsg.InfoQuery, InfoChallengeId));
				}
				stopwatch.Stop();

				Latency = stopwatch.ElapsedMilliseconds;

				return InfoResponse(recvData);
			}
			catch (Exception ex)
			{
				ex.Data.Add("ReceivedData", recvData ?? (new byte[1]));
				throw;
			}
		}

		private ServerInfo InfoResponse(byte[] data)
		{
			var parser = new Parser(data);
			if (parser.ReadByte() != (byte)ResponseMsgHeader.A2S_INFO)
			{
				throw new InvalidHeaderException("A2S_INFO message header is not valid");
			}

			var serverInfo = new ServerInfo
			{
				IsObsolete = false,
				Protocol = parser.ReadByte(),
				Name = parser.ReadString(),
				Map = parser.ReadString(),
				Directory = parser.ReadString(),
				Description = parser.ReadString(),
				Id = parser.ReadUShort(),
				Players = parser.ReadByte(),
				MaxPlayers = parser.ReadByte(),
				Bots = parser.ReadByte(),
				ServerType = (char)parser.ReadByte() switch
				{
					'l' => GameServerType.Listen,
					'd' => GameServerType.Dedicated,
					'p' => GameServerType.SourceTV,
					_ => GameServerType.Invalid
				},
				Environment = (char)parser.ReadByte() switch
				{
					'l' => GameEnvironment.Linux,
					'w' => GameEnvironment.Windows,
					'm' => GameEnvironment.Mac,
					'o' => GameEnvironment.Mac,
					_ => GameEnvironment.Invalid
				},
				IsPrivate = (parser.ReadByte() > 0),
				IsSecure = (parser.ReadByte() > 0),
				GameVersion = parser.ReadString()
			};

			serverInfo.ExtraInfo = new ExtraInfo();
			if (parser.HasUnParsedBytes)
			{
				byte b = parser.ReadByte();
				serverInfo.ExtraInfo.Port = (ushort)(((b & 0x80) > 0) ? parser.ReadUShort() : 0);
				serverInfo.ExtraInfo.SteamId = (((b & 0x10) > 0) ? parser.ReadULong() : 0);
				if ((b & 0x40) > 0)
				{
					serverInfo.ExtraInfo.SpecInfo = new SourceTVInfo
					{
						Port = parser.ReadUShort(),
						Name = parser.ReadString()
					};
				}
				serverInfo.ExtraInfo.Keywords = (((b & 0x20) > 0) ? parser.ReadString() : String.Empty);
				serverInfo.ExtraInfo.GameId = (((b & 0x10) > 0) ? parser.ReadULong() : 0);
			}

			serverInfo.Address = UdpSocket.Address.ToString();
			serverInfo.Ping = Latency;

			return serverInfo;
		}

		private byte[] GetInfoChallengeId()
		{
			byte[] response = UdpSocket.GetResponse(QueryMsg.InfoQuery);
			try
			{
				var parser = new Parser(response);
				switch (parser.ReadByte())
				{
					case (byte)ResponseMsgHeader.A2S_SERVERQUERY_GETCHALLENGE:
						IsInfoChallengeId = true;
						return parser.GetUnParsedBytes();
					case (byte)ResponseMsgHeader.A2S_INFO:
						IsInfoChallengeId = false;
						return response;
					default:
						throw new InvalidHeaderException("A2S_SERVERQUERY_GETCHALLENGE message header is not valid");
				}
			}
			catch (Exception ex)
			{
				ex.Data.Add("ReceivedData", (response == null) ? new byte[1] : response);
				throw;
			}
		}

		/// <summary>
		/// Retrieves information about the players currently on the server.
		/// </summary>
		/// <param name="callback">called on every attempt made to connect to server(max. attempts = Retries + 1).</param>
		/// <returns>Collection of <see cref="T:ValveQuery.GameServer.PlayerInfo" /> instances.</returns>
		public virtual QueryCollection<PlayerInfo> GetPlayers(AttemptCallback callback = null)
		{
			ThrowIfDisposed();
			return Invoke(InternalGetPlayers, ConInfo.Retries + 1, callback, ConInfo.ThrowExceptions);
		}

		private QueryCollection<PlayerInfo> InternalGetPlayers()
		{
			byte[] recvData = null;
			List<PlayerInfo> players;
			try
			{
				if (PlayerChallengeId == null)
				{
					recvData = GetPlayerChallengeId();
					if (IsPlayerChallengeId)
					{
						PlayerChallengeId = recvData;
					}
				}
				if (IsPlayerChallengeId)
				{
					recvData = UdpSocket.GetResponse(Util.MergeByteArrays(QueryMsg.PlayerQuery, PlayerChallengeId));
				}

				var parser = new Parser(recvData);
				if (parser.ReadByte() != (byte)ResponseMsgHeader.A2S_PLAYER)
				{
					throw new InvalidHeaderException("A2S_PLAYER message header is not valid");
				}

				int playerCount = parser.ReadByte();
				players = new List<PlayerInfo>(playerCount);
				for (int i = 0; i < playerCount; i++)
				{
					parser.ReadByte();
					players.Add(new PlayerInfo()
					{
						Name = parser.ReadString(),
						Score = parser.ReadInt(),
						Time = TimeSpan.FromSeconds(parser.ReadFloat())
					});
				}
				if (playerCount == 1 && players[0].Name == "Max Players")
				{
					players.Clear();
				}
			}
			catch (Exception ex)
			{
				ex.Data.Add("ReceivedData", (recvData == null) ? new byte[1] : recvData);
				throw;
			}

			return new QueryCollection<PlayerInfo>(players);
		}

		private byte[] GetPlayerChallengeId()
		{
			byte[] response = UdpSocket.GetResponse(QueryMsg.PlayerChallengeQuery);
			try
			{
				var parser = new Parser(response);
				switch (parser.ReadByte())
				{
					case (byte)ResponseMsgHeader.A2S_SERVERQUERY_GETCHALLENGE:
						IsPlayerChallengeId = true;
						return parser.GetUnParsedBytes();
					case (byte)ResponseMsgHeader.A2S_PLAYER:
						IsPlayerChallengeId = false;
						return response;
					default:
						throw new InvalidHeaderException("A2S_SERVERQUERY_GETCHALLENGE message header is not valid");
				}
			}
			catch (Exception ex)
			{
				ex.Data.Add("ReceivedData", (response == null) ? new byte[1] : response);
				throw;
			}
		}

		/// <summary>
		/// Retrieves server rules.
		/// </summary>
		/// <param name="callback">called on every attempt made to connect to server(max. attempts = <see cref="P:ValveQuery.ConnectionInfo.Retries" /> + 1).</param>
		/// <returns>Collection of <see cref="T:ValveQuery.GameServer.Rule" /> instances.</returns>
		public virtual QueryCollection<Rule> GetRules(AttemptCallback callback = null)
		{
			ThrowIfDisposed();
			return Invoke(InternalGetRules, ConInfo.Retries + 1, callback, ConInfo.ThrowExceptions);
		}

		private QueryCollection<Rule> InternalGetRules()
		{
			byte[] recvData = null;
			List<Rule> rules;
			try
			{
				if (RuleChallengeId == null)
				{
					recvData = GetRuleChallengeId();
					if (IsRuleChallengeId)
					{
						RuleChallengeId = recvData;
					}
				}
				if (IsRuleChallengeId)
				{
					recvData = UdpSocket.GetResponse(Util.MergeByteArrays(QueryMsg.RuleQuery, RuleChallengeId));
				}

				var parser = new Parser(recvData);
				if (parser.ReadByte() != (byte)ResponseMsgHeader.A2S_RULES)
				{
					throw new InvalidHeaderException("A2S_RULES message header is not valid");
				}

				int ruleCount = parser.ReadUShort();
				rules = new List<Rule>(ruleCount);
				for (int i = 0; i < ruleCount; i++)
				{
					rules.Add(new Rule() { Name = parser.ReadString(), Value = parser.ReadString() });
				}
			}
			catch (Exception ex)
			{
				ex.Data.Add("ReceivedData", (recvData == null) ? new byte[1] : recvData);
				throw;
			}

			return new QueryCollection<Rule>(rules);
		}

		private byte[] GetRuleChallengeId()
		{
			byte[] response = UdpSocket.GetResponse(QueryMsg.RuleChallengeQuery);
			try
			{
				var parser = new Parser(response);
				switch (parser.ReadByte())
				{
					case (byte)ResponseMsgHeader.A2S_SERVERQUERY_GETCHALLENGE:
						IsRuleChallengeId = true;
						return BitConverter.GetBytes(parser.ReadInt());
					case (byte)ResponseMsgHeader.A2S_RULES:
						IsRuleChallengeId = false;
						return response;
					default:
						throw new InvalidHeaderException("A2S_SERVERQUERY_GETCHALLENGE message header is not valid");
				}
			}
			catch (Exception ex)
			{
				ex.Data.Add("ReceivedData", (response == null) ? new byte[1] : response);
				throw;
			}
		}

		/// <summary>
		/// Listen to server logs.
		/// </summary>
		/// <param name="port">Local port.</param>
		/// <returns>Instance of <see cref="T:ValveQuery.GameServer.Logs" /> class.</returns>
		/// <remarks>Receiver's socket address must be added to server's logaddress list before listening.</remarks>
		public virtual Logs GetLogs(int port)
		{
			ThrowIfDisposed();
			if (Logs == null)
			{
				Logs = new Logs(port, EndPoint);
			}
			return Logs;
		}

		/// <summary>
		/// Gets valid rcon instance that can be used to send rcon commands to server..
		/// </summary>
		/// <param name="pass">Rcon password of server.</param>
		/// <param name="useWebRcon">Use a web based connection?</param>
		/// <returns>true if server accepted rcon password.</returns>
		public virtual bool GetControl(string pass, bool useWebRcon)
		{
			ThrowIfDisposed();

			Rcon = useWebRcon ? RconWeb.Authorize(ConInfo, pass) : Rcon.Authorize(ConInfo, pass);
			if (Rcon != null)
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Gets round-trip delay time.
		/// </summary>
		/// <returns>Elapsed milliseconds(-1 if server is not responding).</returns>
		public virtual long Ping()
		{
			ThrowIfDisposed();

			var stopwatch = Stopwatch.StartNew();

			try
			{
				UdpSocket.GetResponse(QueryMsg.InfoQuery);
			}
			catch (SocketException)
			{
				stopwatch.Stop();
				return -1L;
			}

			stopwatch.Stop();

			return stopwatch.ElapsedMilliseconds;
		}

		/// <summary>
		/// Disposes all the resources used by this instance.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			if (IsDisposed)
			{
				return;
			}

			if (disposing)
			{
				Rcon?.Dispose();
				Rcon = null;

				Logs?.Dispose();
				Logs = null;

				UdpSocket?.Dispose();
				UdpSocket = null;
			}

			base.Dispose(disposing);
			IsDisposed = true;
		}
	}
}

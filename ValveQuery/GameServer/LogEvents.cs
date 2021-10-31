using System;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;

namespace ValveQuery.GameServer
{
	/// <summary>
	/// Provides mechanism to subscribe and filter logged events.
	/// </summary>
	public class LogEvents : QueryBase
	{
		internal IPEndPoint ServerEndPoint;

		internal Regex LineSplit = new Regex(": ");

		internal Regex RegPlayer = new Regex("^([^\"]+)<([^\"]+)><([^\"]+)><([^\"]*)>$");

		internal Regex RegIsPlayerMsg = new Regex("^.*<\\d+><.+><.*>.*$");

		internal char[] QuoteSplitPattern = new char[1] { '"' };

		/// <summary>
		/// Represents a collection of filters.
		/// </summary>
		public LogFilterCollection Filters { get; set; }

		/// <summary>
		/// Occurs when Server cvar starts(In TFC, if tfc_clanbattle is 1, this doesn't happen.).
		/// </summary>
		public event EventHandler<LogEventArgs> CvarStartMsg;

		/// <summary>
		/// Occurs when someone changes a cvar over rcon.
		/// </summary>
		public event EventHandler<CvarEventArgs> ServerCvar;

		/// <summary>
		/// Occurs when Server cvar ends(In TFC, if tfc_clanbattle is 0, this doesn't happen.).
		/// </summary>
		public event EventHandler<LogEventArgs> CvarEndMsg;

		/// <summary>
		/// Occurs when Logging to file is started.
		/// </summary>
		public event EventHandler<LogStartEventArgs> LogFileStarted;

		/// <summary>
		/// Occurs when Log file is closed.
		/// </summary>
		public event EventHandler<LogEventArgs> LogFileClosed;

		/// <summary>
		/// Occurs when map is loaded.
		/// </summary>
		public event EventHandler<MapLoadEventArgs> MapLoaded;

		/// <summary>
		/// Occurs when Map starts.
		/// </summary>
		public event EventHandler<MapStartEventArgs> MapStarted;

		/// <summary>
		/// Occurs when an rcon message is sent to server.
		/// </summary>
		public event EventHandler<RconEventArgs> RconMsg;

		/// <summary>
		/// Occurs when server name is displayed.
		/// </summary>
		public event EventHandler<ServerNameEventArgs> ServerName;

		/// <summary>
		/// Occurs when Server says.
		/// </summary>
		public event EventHandler<ServerSayEventArgs> ServerSay;

		/// <summary>
		/// Occurs when a player is connected.
		/// </summary>
		public event EventHandler<ConnectEventArgs> PlayerConnected;

		/// <summary>
		/// Occurs when a player is validated.
		/// </summary>
		public event EventHandler<PlayerEventArgs> PlayerValidated;

		/// <summary>
		/// Occurs when a player is enters game.
		/// </summary>
		public event EventHandler<PlayerEventArgs> PlayerEnteredGame;

		/// <summary>
		/// Occurs when a player is disconnected.
		/// </summary>
		public event EventHandler<PlayerEventArgs> PlayerDisconnected;

		/// <summary>
		/// Occurs when a player is kicked.
		/// </summary>
		public event EventHandler<KickEventArgs> PlayerKicked;

		/// <summary>
		/// Occurs when a player commit suicide.
		/// </summary>
		public event EventHandler<SuicideEventArgs> PlayerSuicided;

		/// <summary>
		/// Occurs when a player Join team.
		/// </summary>
		public event EventHandler<TeamSelectionEventArgs> PlayerJoinedTeam;

		/// <summary>
		/// Occurs when a player change role.
		/// </summary>
		public event EventHandler<RoleSelectionEventArgs> PlayerChangedRole;

		/// <summary>
		/// Occurs when a player changes name.
		/// </summary>
		public event EventHandler<NameChangeEventArgs> PlayerChangedName;

		/// <summary>
		/// Occurs when a player is killed.
		/// </summary>
		public event EventHandler<KillEventArgs> PlayerKilled;

		/// <summary>
		/// Occurs when a player is injured.
		/// </summary>
		public event EventHandler<InjureEventArgs> PlayerInjured;

		/// <summary>
		/// Occurs when a player triggers  something on another player(in TFC this event may cover medic healings and infections, sentry gun destruction, spy uncovering.etc).
		/// </summary>
		public event EventHandler<PlayerOnPlayerEventArgs> PlayerOnPLayerTriggered;

		/// <summary>
		///  Occurs when a player triggers an action.
		/// </summary>
		public event EventHandler<PlayerActionEventArgs> PlayerTriggered;

		/// <summary>
		///  Occurs when a team triggers an action(eg:team winning).
		/// </summary>
		public event EventHandler<TeamActionEventArgs> TeamTriggered;

		/// <summary>
		///  Occurs when server triggers an action(eg:roundstart,game events).
		/// </summary>
		public event EventHandler<WorldActionEventArgs> WorldTriggered;

		/// <summary>
		///  Occurs when a player says. 
		/// </summary>
		public event EventHandler<ChatEventArgs> Say;

		/// <summary>
		///  Occurs when a player uses teamsay.
		/// </summary>
		public event EventHandler<ChatEventArgs> TeamSay;

		/// <summary>
		///  Occurs when a team forms alliance with another team.
		/// </summary>
		public event EventHandler<TeamAllianceEventArgs> TeamAlliance;

		/// <summary>
		///  Occurs when Team Score Report is displayed at round end.
		/// </summary>
		public event EventHandler<TeamScoreReportEventArgs> TeamScoreReport;

		/// <summary>
		/// Occurs when a private message is sent.
		/// </summary>
		public event EventHandler<PrivateChatEventArgs> PrivateChat;

		/// <summary>
		/// Occurs when Player Score Report is displayed at round end.
		/// </summary>
		public event EventHandler<PlayerScoreReportEventArgs> PlayerScoreReport;

		/// <summary>
		/// Occurs when Player selects a weapon.
		/// </summary>
		public event EventHandler<WeaponEventArgs> PlayerSelectedWeapon;

		/// <summary>
		/// Occurs when Player acquires a weapon.
		/// </summary>
		public event EventHandler<WeaponEventArgs> PlayerAcquiredWeapon;

		/// <summary>
		/// Occurs when server shuts down.
		/// </summary>
		public event EventHandler<LogEventArgs> Shutdown;

		/// <summary>
		/// Occurs when a log message cannot be parsed.
		/// </summary>
		public event EventHandler<ExceptionEventArgs> Exception;

		/// <summary>
		/// Occurs when a log message is received.
		/// </summary>
		public event EventHandler<LogReceivedEventArgs> LogReceived;

		/// <summary>
		/// Occurs when a log comment is received.
		/// </summary>
		public event EventHandler<CommentReceivedEventArgs> CommentReceived;

		/// <summary>
		/// Initializes LogEvents.
		/// </summary>
		/// <param name="endPoint">server EndPoint.</param>
		protected internal LogEvents(IPEndPoint endPoint)
		{
			ServerEndPoint = endPoint;
			Filters = new LogFilterCollection();
		}

		/// <summary>
		/// Processes received log messages.
		/// </summary>
		/// <param name="logLine"></param>
		protected internal void ProcessLog(string logLine)
		{
			ThrowIfDisposed();
			DateTime timestamp;
			string info;
			try
			{
				string[] data = LineSplit.Split(logLine, 2);
				timestamp = DateTime.ParseExact(data[0], "MM/dd/yyyy - HH:mm:ss", CultureInfo.InvariantCulture);
				info = data[1].Remove(data[1].Length - 2);
			}
			catch (Exception e)
			{
				e.Data.Add("ReceivedData", Util.StringToBytes(logLine));
				throw;
			}

			info = ApplyFilters(info);
			if (String.IsNullOrEmpty(info))
			{
				return;
			}

			if (info.StartsWith("//", StringComparison.OrdinalIgnoreCase))
			{
				OnCommentReceive(timestamp, info);
			}

			OnLogReceive(timestamp, info);

			string[] result = info.Split(QuoteSplitPattern, StringSplitOptions.RemoveEmptyEntries);
			try
			{
				if (info[0] == '"')
				{
					switch (result[1])
					{
						case " connected, address ":
							OnConnection(timestamp, result);
							break;
						case " STEAM USERID validated":
							OnValidation(timestamp, result);
							break;
						case " entered the game":
							OnEnterGame(timestamp, result);
							break;
						case " disconnected":
							OnDisconnection(timestamp, result);
							break;
						case " committed suicide with ":
							OnSuicide(timestamp, result);
							break;
						case " joined team ":
							OnTeamSelection(timestamp, result);
							break;
						case " changed role to ":
							OnRoleSelection(timestamp, result);
							break;
						case " changed name to ":
							OnNameChange(timestamp, result);
							break;
						case " killed ":
							OnKill(timestamp, result);
							break;
						case " attacked ":
							OnInjure(timestamp, result);
							break;
						case " triggered ":
							if (result.Length > 3 && result[3] == " against ")
							{
								OnPlayer_PlayerAction(timestamp, result);
							}
							else
							{
								OnPlayerAction(timestamp, result);
							}
							break;
						case " say ":
							OnSay(timestamp, result);
							break;
						case " say_team ":
							OnTeamSay(timestamp, result);
							break;
						case " tell ":
							OnPrivateChat(timestamp, result);
							break;
						case " selected weapon ":
							OnWeaponSelection(timestamp, result);
							break;
						case " acquired weapon ":
							OnWeaponPickup(timestamp, result);
							break;
						default:
							OnException(timestamp, info);
							break;
					}
				}
				else
				{
					switch (result[0])
					{
						case "Server cvars start":
							OnCvarStart(timestamp);
							break;
						case "Server cvar ":
							OnServerCvar(timestamp, result);
							break;
						case "Server cvars end":
							OnCvarEnd(timestamp);
							break;
						case "Log file started (file ":
							OnLogFileStart(timestamp, result);
							break;
						case "Log file closed":
							OnLogFileClose(timestamp);
							break;
						case "Loading map ":
							OnMapLoading(timestamp, result);
							break;
						case "Started map ":
							OnMapStart(timestamp, result);
							break;
						case "Rcon: ":
							OnRconMsg(timestamp, result);
							break;
						case "Bad Rcon: ":
							OnRconMsg(timestamp, result);
							break;
						case "Server name is ":
							OnserverName(timestamp, result);
							break;
						case "Server say ":
							OnServerSay(timestamp, result);
							break;
						case "Kick: ":
							OnKick(timestamp, result);
							break;
						case "Team ":
						{
							switch (result[2])
							{
								case " triggered ":
									OnTeamAction(timestamp, result);
									break;
								case " formed alliance with team ":
									OnTeamAlliance(timestamp, result);
									break;
								case " scored ":
									OnTeamScoreReport(timestamp, result);
									break;
							}

							break;
						}
						case "World triggered ":
							OnWorldAction(timestamp, result);
							break;
						case "Player ":
							OnPlayerAction(timestamp, result);
							break;
						case "Server shutdown":
							OnShutdown(timestamp);
							break;
						default:
							OnException(timestamp, info);
							break;
					}
				}
			}
			catch (Exception)
			{
				Exception.Fire(ServerEndPoint, new ExceptionEventArgs
				{
					Timestamp = timestamp,
					Message = info
				});
			}
		}

		private string ApplyFilters(string logLine)
		{
			ThrowIfDisposed();

			foreach (LogFilter filter in Filters)
			{
				if (filter.Enabled)
				{
					if (filter.RegexInstance == null)
					{
						filter.RegexInstance = new Regex(filter.ToString());
					}
					switch (filter.Action)
					{
						case LogFilterAction.Allow:
							if (RegIsPlayerMsg.IsMatch(logLine) && !filter.RegexInstance.IsMatch(logLine))
							{
								logLine = String.Empty;
							}
							break;
						case LogFilterAction.Block:
							if (RegIsPlayerMsg.IsMatch(logLine) && filter.RegexInstance.IsMatch(logLine))
							{
								logLine = String.Empty;
							}
							break;
					}
				}
			}

			return logLine;
		}

		/// <summary>
		/// Disposes all the resources used by this instance.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				if (disposing)
				{
					CvarStartMsg = null;
					ServerCvar = null;
					CvarEndMsg = null;
					LogFileStarted = null;
					LogFileClosed = null;
					MapLoaded = null;
					MapStarted = null;
					RconMsg = null;
					ServerName = null;
					ServerSay = null;
					PlayerConnected = null;
					PlayerValidated = null;
					PlayerEnteredGame = null;
					PlayerDisconnected = null;
					PlayerKicked = null;
					PlayerSuicided = null;
					PlayerJoinedTeam = null;
					PlayerChangedRole = null;
					PlayerChangedName = null;
					PlayerKilled = null;
					PlayerInjured = null;
					PlayerOnPLayerTriggered = null;
					PlayerTriggered = null;
					TeamTriggered = null;
					WorldTriggered = null;
					Say = null;
					TeamSay = null;
					TeamAlliance = null;
					TeamScoreReport = null;
					PrivateChat = null;
					PlayerScoreReport = null;
					PlayerSelectedWeapon = null;
					PlayerAcquiredWeapon = null;
					Shutdown = null;
					Exception = null;
				}

				base.Dispose(disposing);
				IsDisposed = true;
			}
		}

		/// <summary>
		/// Raises the <see cref="E:ValveQuery.GameServer.LogEvents.CommentReceived" /> event.
		/// </summary>
		/// <param name="timestamp">Time at which <see cref="E:ValveQuery.GameServer.LogEvents.CommentReceived" /> event was fired.</param>
		/// <param name="message">Comment line.</param>
		protected virtual void OnCommentReceive(DateTime timestamp, string message)
		{
			CommentReceived.Fire(ServerEndPoint, new CommentReceivedEventArgs
			{
				Timestamp = timestamp,
				Comment = message.Remove(0, 2)
			});
		}

		/// <summary>
		/// Raises the <see cref="E:ValveQuery.GameServer.LogEvents.LogReceived" /> event.
		/// </summary>
		/// <param name="timestamp">Time at which log message was received.</param>
		/// <param name="message">Received log line.</param>
		protected virtual void OnLogReceive(DateTime timestamp, string message)
		{
			LogReceived.Fire(ServerEndPoint, new LogReceivedEventArgs
			{
				Timestamp = timestamp,
				Message = message
			});
		}

		/// <summary>
		/// Raises the <see cref="E:ValveQuery.GameServer.LogEvents.CvarStartMsg" /> event.
		/// </summary>
		/// <param name="timestamp">Time at which <see cref="E:ValveQuery.GameServer.LogEvents.CvarStartMsg" /> event was fired.</param>
		protected virtual void OnCvarStart(DateTime timestamp)
		{
			CvarStartMsg.Fire(ServerEndPoint, new LogEventArgs
			{
				Timestamp = timestamp
			});
		}

		/// <summary>
		/// Raises the <see cref="E:ValveQuery.GameServer.LogEvents.ServerCvar" /> event.
		/// </summary>
		/// <param name="timestamp">Time at which <see cref="E:ValveQuery.GameServer.LogEvents.ServerCvar" /> event was fired.</param>
		/// <param name="info">Information about <see cref="E:ValveQuery.GameServer.LogEvents.ServerCvar" /> event.</param>
		protected virtual void OnServerCvar(DateTime timestamp, string[] info)
		{
			ServerCvar.Fire(ServerEndPoint, new CvarEventArgs
			{
				Timestamp = timestamp,
				Cvar = info[1],
				Value = info[3]
			});
		}

		/// <summary>
		/// Raises the <see cref="E:ValveQuery.GameServer.LogEvents.CvarEndMsg" /> event.
		/// </summary>
		/// <param name="timestamp">Time at which <see cref="E:ValveQuery.GameServer.LogEvents.CvarEndMsg" /> event was fired.</param>
		protected virtual void OnCvarEnd(DateTime timestamp)
		{
			CvarEndMsg.Fire(ServerEndPoint, new LogEventArgs
			{
				Timestamp = timestamp
			});
		}

		/// <summary>
		/// Raises the <see cref="E:ValveQuery.GameServer.LogEvents.LogFileStarted" /> event.
		/// </summary>
		/// <param name="timestamp">Time at which <see cref="E:ValveQuery.GameServer.LogEvents.LogFileStarted" /> event was fired.</param>
		/// <param name="info">Information about <see cref="E:ValveQuery.GameServer.LogEvents.LogFileStarted" /> event.</param>
		protected virtual void OnLogFileStart(DateTime timestamp, string[] info)
		{
			string[] tmp = info[5].Split('/');
			LogFileStarted.Fire(ServerEndPoint, new LogStartEventArgs()
			{
				Timestamp = timestamp,
				FileName = info[1],
				Game = info[3],
				Protocol = tmp[0],
				Release = tmp[1],
				Build = tmp[2]
			});
		}

		/// <summary>
		/// Raises the <see cref="E:ValveQuery.GameServer.LogEvents.LogFileClosed" /> event.
		/// </summary>
		/// <param name="timestamp">Time at which <see cref="E:ValveQuery.GameServer.LogEvents.LogFileClosed" /> event was fired.</param>
		protected virtual void OnLogFileClose(DateTime timestamp)
		{
			LogFileClosed.Fire(ServerEndPoint, new LogEventArgs
			{
				Timestamp = timestamp
			});
		}

		/// <summary>
		/// Raises the <see cref="E:ValveQuery.GameServer.LogEvents.MapLoaded" /> event.
		/// </summary>
		/// <param name="timestamp">Time at which <see cref="E:ValveQuery.GameServer.LogEvents.MapLoaded" /> event was fired.</param>
		/// <param name="info">Information about <see cref="E:ValveQuery.GameServer.LogEvents.MapLoaded" /> event.</param>
		protected virtual void OnMapLoading(DateTime timestamp, string[] info)
		{
			MapLoaded.Fire(ServerEndPoint, new MapLoadEventArgs
			{
				Timestamp = timestamp,
				MapName = info[1]
			});
		}

		/// <summary>
		/// Raises the <see cref="E:ValveQuery.GameServer.LogEvents.MapStarted" /> event.
		/// </summary>
		/// <param name="timestamp">Time at which <see cref="E:ValveQuery.GameServer.LogEvents.MapStarted" /> event was fired.</param>
		/// <param name="info">Information about <see cref="E:ValveQuery.GameServer.LogEvents.MapStarted" /> event.</param>
		protected virtual void OnMapStart(DateTime timestamp, string[] info)
		{
			MapStarted.Fire(ServerEndPoint, new MapStartEventArgs
			{
				Timestamp = timestamp,
				MapName = info[1],
				MapCRC = info[3]
			});
		}

		/// <summary>
		/// Raises the <see cref="E:ValveQuery.GameServer.LogEvents.RconMsg" /> event.
		/// </summary>
		/// <param name="timestamp">Time at which <see cref="E:ValveQuery.GameServer.LogEvents.RconMsg" /> event was fired.</param>
		/// <param name="info">Information about <see cref="E:ValveQuery.GameServer.LogEvents.RconMsg" /> event.</param>
		protected virtual void OnRconMsg(DateTime timestamp, string[] info)
		{
			string[] tmp = info[5].Split(':');
			RconMsg.Fire(ServerEndPoint, new RconEventArgs
			{
				Timestamp = timestamp,
				IsValid = ((info[0] == "Rcon: ") ? true : false),
				Challenge = info[1].Split(' ')[1],
				Password = info[2],
				Command = info[3],
				Ip = tmp[0],
				Port = UInt16.Parse(tmp[1], CultureInfo.InvariantCulture)
			});
		}

		/// <summary>
		/// Raises the <see cref="E:ValveQuery.GameServer.LogEvents.ServerName" /> event.
		/// </summary>
		/// <param name="timestamp">Time at which <see cref="E:ValveQuery.GameServer.LogEvents.ServerName" /> event was fired.</param>
		/// <param name="info">Information about <see cref="E:ValveQuery.GameServer.LogEvents.ServerName" /> event.</param>
		protected virtual void OnserverName(DateTime timestamp, string[] info)
		{
			ServerName.Fire(ServerEndPoint, new ServerNameEventArgs
			{
				Timestamp = timestamp,
				Name = info[1]
			});
		}

		/// <summary>
		/// Raises the <see cref="E:ValveQuery.GameServer.LogEvents.ServerSay" /> event.
		/// </summary>
		/// <param name="timestamp">Time at which <see cref="E:ValveQuery.GameServer.LogEvents.ServerSay" /> event was fired.</param>
		/// <param name="info">Information about <see cref="E:ValveQuery.GameServer.LogEvents.ServerSay" /> event.</param>
		protected virtual void OnServerSay(DateTime timestamp, string[] info)
		{
			ServerSay.Fire(ServerEndPoint, new ServerSayEventArgs
			{
				Timestamp = timestamp,
				Message = info[1]
			});
		}

		/// <summary>
		/// Raises the <see cref="E:ValveQuery.GameServer.LogEvents.PlayerConnected" /> event.
		/// </summary>
		/// <param name="timestamp">Time at which <see cref="E:ValveQuery.GameServer.LogEvents.PlayerConnected" /> event was fired.</param>
		/// <param name="info">Information about <see cref="E:ValveQuery.GameServer.LogEvents.PlayerConnected" /> event.</param>
		protected virtual void OnConnection(DateTime timestamp, string[] info)
		{
			string[] tmp = info[2].Split(':');
			PlayerConnected.Fire(ServerEndPoint, new ConnectEventArgs
			{
				Timestamp = timestamp,
				Player = GetPlayerInfo(info[0]),
				Ip = tmp[0],
				Port = UInt16.Parse(tmp[1], CultureInfo.InvariantCulture)
			});
		}

		/// <summary>
		/// Raises the <see cref="E:ValveQuery.GameServer.LogEvents.PlayerValidated" /> event.
		/// </summary>
		/// <param name="timestamp">Time at which <see cref="E:ValveQuery.GameServer.LogEvents.PlayerValidated" /> event was fired.</param>
		/// <param name="info">Information about <see cref="E:ValveQuery.GameServer.LogEvents.PlayerValidated" /> event.</param>
		protected virtual void OnValidation(DateTime timestamp, string[] info)
		{
			PlayerValidated.Fire(ServerEndPoint, new PlayerEventArgs
			{
				Timestamp = timestamp,
				Player = GetPlayerInfo(info[0])
			});
		}

		/// <summary>
		/// Raises the <see cref="E:ValveQuery.GameServer.LogEvents.PlayerEnteredGame" /> event.
		/// </summary>
		/// <param name="timestamp">Time at which <see cref="E:ValveQuery.GameServer.LogEvents.PlayerEnteredGame" /> event was fired.</param>
		/// <param name="info">Information about <see cref="E:ValveQuery.GameServer.LogEvents.PlayerEnteredGame" /> event.</param>
		protected virtual void OnEnterGame(DateTime timestamp, string[] info)
		{
			PlayerEnteredGame.Fire(ServerEndPoint, new PlayerEventArgs
			{
				Timestamp = timestamp,
				Player = GetPlayerInfo(info[0])
			});
		}

		/// <summary>
		/// Raises the <see cref="E:ValveQuery.GameServer.LogEvents.PlayerDisconnected" /> event.
		/// </summary>
		/// <param name="timestamp">Time at which <see cref="E:ValveQuery.GameServer.LogEvents.PlayerDisconnected" /> event was fired.</param>
		/// <param name="info">Information about <see cref="E:ValveQuery.GameServer.LogEvents.PlayerDisconnected" /> event.</param>
		protected virtual void OnDisconnection(DateTime timestamp, string[] info)
		{
			PlayerDisconnected.Fire(ServerEndPoint, new PlayerEventArgs
			{
				Timestamp = timestamp,
				Player = GetPlayerInfo(info[0])
			});
		}

		/// <summary>
		/// Raises the <see cref="E:ValveQuery.GameServer.LogEvents.PlayerKicked" /> event.
		/// </summary>
		/// <param name="timestamp">Time at which <see cref="E:ValveQuery.GameServer.LogEvents.PlayerKicked" /> event was fired.</param>
		/// <param name="info">Information about <see cref="E:ValveQuery.GameServer.LogEvents.PlayerKicked" /> event.</param>
		protected virtual void OnKick(DateTime timestamp, string[] info)
		{
			PlayerKicked.Fire(ServerEndPoint, new KickEventArgs
			{
				Timestamp = timestamp,
				Player = GetPlayerInfo(info[1]),
				Kicker = info[3],
				Message = ((info.Length == 7) ? info[5] : String.Empty)
			});
		}

		/// <summary>
		/// Raises the <see cref="E:ValveQuery.GameServer.LogEvents.PlayerSuicided" /> event.
		/// </summary>
		/// <param name="timestamp">Time at which <see cref="E:ValveQuery.GameServer.LogEvents.PlayerSuicided" /> event was fired.</param>
		/// <param name="info">Information about <see cref="E:ValveQuery.GameServer.LogEvents.PlayerSuicided" /> event.</param>
		protected virtual void OnSuicide(DateTime timestamp, string[] info)
		{
			PlayerSuicided.Fire(ServerEndPoint, new SuicideEventArgs
			{
				Timestamp = timestamp,
				Player = GetPlayerInfo(info[0]),
				Weapon = info[2]
			});
		}

		/// <summary>
		/// Raises the <see cref="E:ValveQuery.GameServer.LogEvents.PlayerJoinedTeam" /> event.
		/// </summary>
		/// <param name="timestamp">Time at which <see cref="E:ValveQuery.GameServer.LogEvents.PlayerJoinedTeam" /> event was fired.</param>
		/// <param name="info">Information about <see cref="E:ValveQuery.GameServer.LogEvents.PlayerJoinedTeam" /> event.</param>
		protected virtual void OnTeamSelection(DateTime timestamp, string[] info)
		{
			PlayerJoinedTeam.Fire(ServerEndPoint, new TeamSelectionEventArgs
			{
				Timestamp = timestamp,
				Player = GetPlayerInfo(info[0]),
				Team = info[2]
			});
		}

		/// <summary>
		/// Raises the <see cref="E:ValveQuery.GameServer.LogEvents.PlayerChangedRole" /> event.
		/// </summary>
		/// <param name="timestamp">Time at which <see cref="E:ValveQuery.GameServer.LogEvents.PlayerChangedRole" /> event was fired.</param>
		/// <param name="info">Information about <see cref="E:ValveQuery.GameServer.LogEvents.PlayerChangedRole" /> event.</param>
		protected virtual void OnRoleSelection(DateTime timestamp, string[] info)
		{
			PlayerChangedRole.Fire(ServerEndPoint, new RoleSelectionEventArgs
			{
				Timestamp = timestamp,
				Player = GetPlayerInfo(info[0]),
				Role = info[2]
			});
		}

		/// <summary>
		/// Raises the <see cref="E:ValveQuery.GameServer.LogEvents.PlayerChangedName" /> event.
		/// </summary>
		/// <param name="timestamp">Time at which <see cref="E:ValveQuery.GameServer.LogEvents.PlayerChangedName" /> event was fired.</param>
		/// <param name="info">Information about <see cref="E:ValveQuery.GameServer.LogEvents.PlayerChangedName" /> event.</param>
		protected virtual void OnNameChange(DateTime timestamp, string[] info)
		{
			PlayerChangedName.Fire(ServerEndPoint, new NameChangeEventArgs
			{
				Timestamp = timestamp,
				Player = GetPlayerInfo(info[0]),
				NewName = info[2]
			});
		}

		/// <summary>
		/// Raises the <see cref="E:ValveQuery.GameServer.LogEvents.PlayerKilled" /> event.
		/// </summary>
		/// <param name="timestamp">Time at which <see cref="E:ValveQuery.GameServer.LogEvents.PlayerKilled" /> event was fired.</param>
		/// <param name="info">Information about <see cref="E:ValveQuery.GameServer.LogEvents.PlayerKilled" /> event.</param>
		protected virtual void OnKill(DateTime timestamp, string[] info)
		{
			PlayerKilled.Fire(ServerEndPoint, new KillEventArgs
			{
				Timestamp = timestamp,
				Player = GetPlayerInfo(info[0]),
				Victim = GetPlayerInfo(info[2]),
				Weapon = info[4]
			});
		}

		/// <summary>
		/// Raises the <see cref="E:ValveQuery.GameServer.LogEvents.PlayerInjured" /> event.
		/// </summary>
		/// <param name="timestamp">Time at which <see cref="E:ValveQuery.GameServer.LogEvents.PlayerInjured" /> event was fired.</param>
		/// <param name="info">Information about <see cref="E:ValveQuery.GameServer.LogEvents.PlayerInjured" /> event.</param>
		protected virtual void OnInjure(DateTime timestamp, string[] info)
		{
			PlayerInjured.Fire(ServerEndPoint, new InjureEventArgs
			{
				Timestamp = timestamp,
				Player = GetPlayerInfo(info[0]),
				Victim = GetPlayerInfo(info[2]),
				Weapon = info[4],
				Damage = info[6]
			});
		}

		/// <summary>
		/// Raises the <see cref="E:ValveQuery.GameServer.LogEvents.PlayerOnPLayerTriggered" /> event.
		/// </summary>
		/// <param name="timestamp">Time at which <see cref="E:ValveQuery.GameServer.LogEvents.PlayerOnPLayerTriggered" /> event was fired.</param>
		/// <param name="info">Information about <see cref="E:ValveQuery.GameServer.LogEvents.PlayerOnPLayerTriggered" /> event.</param>
		protected virtual void OnPlayer_PlayerAction(DateTime timestamp, string[] info)
		{
			PlayerOnPLayerTriggered.Fire(ServerEndPoint, new PlayerOnPlayerEventArgs
			{
				Timestamp = timestamp,
				Source = GetPlayerInfo(info[0]),
				Action = info[2],
				Target = GetPlayerInfo(info[4])
			});
		}

		/// <summary>
		/// Raises the <see cref="E:ValveQuery.GameServer.LogEvents.PlayerTriggered" /> event.
		/// </summary>
		/// <param name="timestamp">Time at which <see cref="E:ValveQuery.GameServer.LogEvents.PlayerTriggered" /> event was fired.</param>
		/// <param name="info">Information about <see cref="E:ValveQuery.GameServer.LogEvents.PlayerTriggered" /> event.</param>
		protected virtual void OnPlayerAction(DateTime timestamp, string[] info)
		{
			string text = String.Empty;
			if (info.Length > 3)
			{
				for (int i = 3; i < info.Length; i++)
				{
					text += info[i];
				}
			}

			PlayerTriggered.Fire(ServerEndPoint, new PlayerActionEventArgs
			{
				Timestamp = timestamp,
				Player = GetPlayerInfo(info[0]),
				Action = info[2],
				ExtraInfo = text
			});
		}

		/// <summary>
		/// Raises the <see cref="E:ValveQuery.GameServer.LogEvents.TeamTriggered" /> event.
		/// </summary>
		/// <param name="timestamp">Time at which <see cref="E:ValveQuery.GameServer.LogEvents.TeamTriggered" /> event was fired.</param>
		/// <param name="info">Information about <see cref="E:ValveQuery.GameServer.LogEvents.TeamTriggered" /> event.</param>
		protected virtual void OnTeamAction(DateTime timestamp, string[] info)
		{
			TeamTriggered.Fire(ServerEndPoint, new TeamActionEventArgs
			{
				Timestamp = timestamp,
				Team = info[1],
				Action = info[3]
			});
		}

		/// <summary>
		/// Raises the <see cref="E:ValveQuery.GameServer.LogEvents.WorldTriggered" /> event.
		/// </summary>
		/// <param name="timestamp">Time at which <see cref="E:ValveQuery.GameServer.LogEvents.WorldTriggered" /> event was fired.</param>
		/// <param name="info">Information about <see cref="E:ValveQuery.GameServer.LogEvents.WorldTriggered" /> event.</param>
		protected virtual void OnWorldAction(DateTime timestamp, string[] info)
		{
			WorldTriggered.Fire(ServerEndPoint, new WorldActionEventArgs
			{
				Timestamp = timestamp,
				Action = info[1]
			});
		}

		/// <summary>
		/// Raises the <see cref="E:ValveQuery.GameServer.LogEvents.Say" /> event.
		/// </summary>
		/// <param name="timestamp">Time at which <see cref="E:ValveQuery.GameServer.LogEvents.Say" /> event was fired.</param>
		/// <param name="info">Information about <see cref="E:ValveQuery.GameServer.LogEvents.Say" /> event.</param>
		protected virtual void OnSay(DateTime timestamp, string[] info)
		{
			Say.Fire(ServerEndPoint, new ChatEventArgs
			{
				Timestamp = timestamp,
				Player = GetPlayerInfo(info[0]),
				Message = ((info.Length == 3) ? info[2] : String.Empty)
			});
		}

		/// <summary>
		/// Raises the <see cref="E:ValveQuery.GameServer.LogEvents.TeamSay" /> event.
		/// </summary>
		/// <param name="timestamp">Time at which <see cref="E:ValveQuery.GameServer.LogEvents.TeamSay" /> event was fired.</param>
		/// <param name="info">Information about <see cref="E:ValveQuery.GameServer.LogEvents.TeamSay" /> event.</param>
		protected virtual void OnTeamSay(DateTime timestamp, string[] info)
		{
			TeamSay.Fire(ServerEndPoint, new ChatEventArgs
			{
				Timestamp = timestamp,
				Player = GetPlayerInfo(info[0]),
				Message = ((info.Length == 3) ? info[2] : String.Empty)
			});
		}

		/// <summary>
		/// Raises the <see cref="E:ValveQuery.GameServer.LogEvents.TeamAlliance" /> event.
		/// </summary>
		/// <param name="timestamp">Time at which <see cref="E:ValveQuery.GameServer.LogEvents.TeamAlliance" /> event was fired.</param>
		/// <param name="info">Information about <see cref="E:ValveQuery.GameServer.LogEvents.TeamAlliance" /> event.</param>
		protected virtual void OnTeamAlliance(DateTime timestamp, string[] info)
		{
			TeamAlliance.Fire(ServerEndPoint, new TeamAllianceEventArgs
			{
				Timestamp = timestamp,
				Team1 = info[1],
				Team2 = info[3]
			});
		}

		/// <summary>
		/// Raises the <see cref="E:ValveQuery.GameServer.LogEvents.TeamScoreReport" /> event.
		/// </summary>
		/// <param name="timestamp">Time at which <see cref="E:ValveQuery.GameServer.LogEvents.TeamScoreReport" /> event was fired.</param>
		/// <param name="info">Information about <see cref="E:ValveQuery.GameServer.LogEvents.TeamScoreReport" /> event.</param>
		protected virtual void OnTeamScoreReport(DateTime timestamp, string[] info)
		{
			string text = String.Empty;
			if (info.Length > 6)
			{
				for (int i = 6; i < info.Length; i++)
				{
					text += info[i];
				}
			}

			TeamScoreReport.Fire(ServerEndPoint, new TeamScoreReportEventArgs
			{
				Timestamp = timestamp,
				Team = info[1],
				Score = info[3],
				PlayerCount = info[5],
				ExtraInfo = text
			});
		}

		/// <summary>
		/// Raises the <see cref="E:ValveQuery.GameServer.LogEvents.PrivateChat" /> event.
		/// </summary>
		/// <param name="timestamp">Time at which <see cref="E:ValveQuery.GameServer.LogEvents.PrivateChat" /> event was fired.</param>
		/// <param name="info">Information about <see cref="E:ValveQuery.GameServer.LogEvents.PrivateChat" /> event.</param>
		protected virtual void OnPrivateChat(DateTime timestamp, string[] info)
		{
			PrivateChat.Fire(ServerEndPoint, new PrivateChatEventArgs
			{
				Timestamp = timestamp,
				Sender = GetPlayerInfo(info[0]),
				Receiver = GetPlayerInfo(info[2]),
				Message = ((info.Length == 5) ? info[4] : String.Empty)
			});
		}

		/// <summary>
		/// Raises the <see cref="E:ValveQuery.GameServer.LogEvents.PlayerScoreReport" /> event.
		/// </summary>
		/// <param name="timestamp">Time at which <see cref="E:ValveQuery.GameServer.LogEvents.PlayerScoreReport" /> event was fired.</param>
		/// <param name="info">Information about <see cref="E:ValveQuery.GameServer.LogEvents.PlayerScoreReport" /> event.</param>
		protected virtual void OnPlayerScoreReport(DateTime timestamp, string[] info)
		{
			string text = String.Empty;
			if (info.Length > 4)
			{
				for (int i = 4; i < info.Length; i++)
				{
					text += info[i];
				}
			}

			PlayerScoreReport.Fire(ServerEndPoint, new PlayerScoreReportEventArgs
			{
				Timestamp = timestamp,
				Player = GetPlayerInfo(info[1]),
				Score = info[3],
				ExtraInfo = text
			});
		}

		/// <summary>
		/// Raises the <see cref="E:ValveQuery.GameServer.LogEvents.PlayerSelectedWeapon" /> event.
		/// </summary>
		/// <param name="timestamp">Time at which <see cref="E:ValveQuery.GameServer.LogEvents.PlayerSelectedWeapon" /> event was fired.</param>
		/// <param name="info">Information about <see cref="E:ValveQuery.GameServer.LogEvents.PlayerSelectedWeapon" /> event.</param>
		protected virtual void OnWeaponSelection(DateTime timestamp, string[] info)
		{
			PlayerSelectedWeapon.Fire(ServerEndPoint, new WeaponEventArgs
			{
				Timestamp = timestamp,
				Player = GetPlayerInfo(info[0]),
				Weapon = info[2]
			});
		}

		/// <summary>
		/// Raises the <see cref="E:ValveQuery.GameServer.LogEvents.PlayerSelectedWeapon" /> event.
		/// </summary>
		/// <param name="timestamp">Time at which <see cref="E:ValveQuery.GameServer.LogEvents.PlayerSelectedWeapon" /> event was fired.</param>
		/// <param name="info">Information about <see cref="E:ValveQuery.GameServer.LogEvents.PlayerSelectedWeapon" /> event.</param>
		protected virtual void OnWeaponPickup(DateTime timestamp, string[] info)
		{
			PlayerAcquiredWeapon.Fire(ServerEndPoint, new WeaponEventArgs
			{
				Timestamp = timestamp,
				Player = GetPlayerInfo(info[0]),
				Weapon = info[2]
			});
		}

		/// <summary>
		/// Raises the <see cref="E:ValveQuery.GameServer.LogEvents.PlayerSelectedWeapon" /> event.
		/// </summary>
		/// <param name="timestamp">Time at which <see cref="E:ValveQuery.GameServer.LogEvents.PlayerSelectedWeapon" /> event was fired.</param>
		protected virtual void OnShutdown(DateTime timestamp)
		{
			Shutdown.Fire(ServerEndPoint, new LogEventArgs
			{
				Timestamp = timestamp
			});
		}

		/// <summary>
		/// Raises the <see cref="E:ValveQuery.GameServer.LogEvents.Exception" /> event.
		/// </summary>
		/// <param name="timestamp">Time at which <see cref="E:ValveQuery.GameServer.LogEvents.Exception" /> event was fired.</param>
		/// <param name="info">Information about <see cref="E:ValveQuery.GameServer.LogEvents.Exception" /> event.</param>
		protected virtual void OnException(DateTime timestamp, string info)
		{
			Exception.Fire(ServerEndPoint, new ExceptionEventArgs
			{
				Timestamp = timestamp,
				Message = info
			});
		}

		private LogPlayerInfo GetPlayerInfo(string s)
		{
			Match match = RegPlayer.Match(s);
			return new LogPlayerInfo
			{
				Name = match.Groups[1].Value,
				Uid = match.Groups[2].Value,
				WonId = match.Groups[3].Value,
				Team = match.Groups[4].Value
			};
		}
	}
}

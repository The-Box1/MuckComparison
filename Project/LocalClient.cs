using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Steamworks;
using UnityEngine;

public class LocalClient : MonoBehaviour
{
	public delegate void PacketHandler(Packet packet);

	public class TCP
	{
		public TcpClient socket;

		private NetworkStream stream;

		private Packet receivedData;

		private byte[] receiveBuffer;

		public void Connect()
		{
			socket = new TcpClient
			{
				ReceiveBufferSize = dataBufferSize,
				SendBufferSize = dataBufferSize
			};
			receiveBuffer = new byte[dataBufferSize];
			socket.BeginConnect(instance.ip, instance.port, ConnectCallback, socket);
		}

		private void ConnectCallback(IAsyncResult result)
		{
			socket.EndConnect(result);
			if (socket.Connected)
			{
				stream = socket.GetStream();
				receivedData = new Packet();
				stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
			}
		}

		public void SendData(Packet packet)
		{
			try
			{
				if (socket != null)
				{
					stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
				}
			}
			catch (Exception arg)
			{
				Debug.Log($"Error sending data to server via TCP: {arg}");
			}
		}

		private void ReceiveCallback(IAsyncResult result)
		{
			try
			{
				int num = stream.EndRead(result);
				if (num <= 0)
				{
					instance.Disconnect();
					return;
				}
				byte[] array = new byte[num];
				Array.Copy(receiveBuffer, array, num);
				receivedData.Reset(HandleData(array));
				stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
			}
			catch
			{
				Disconnect();
			}
		}

		private bool HandleData(byte[] data)
		{
			packetsReceived++;
			int packetLength = 0;
			receivedData.SetBytes(data);
			if (receivedData.UnreadLength() >= 4)
			{
				packetLength = receivedData.ReadInt();
				if (packetLength <= 0)
				{
					return true;
				}
			}
			while (packetLength > 0 && packetLength <= receivedData.UnreadLength())
			{
				byte[] packetBytes = receivedData.ReadBytes(packetLength);
				ThreadManagerClient.ExecuteOnMainThread(delegate
				{
					using Packet packet = new Packet(packetBytes);
					int num = packet.ReadInt();
					byteDown += packetLength;
					Debug.Log("received packet: " + (ServerPackets)num);
					packetHandlers[num](packet);
				});
				packetLength = 0;
				if (receivedData.UnreadLength() >= 4)
				{
					packetLength = receivedData.ReadInt();
					if (packetLength <= 0)
					{
						return true;
					}
				}
			}
			if (packetLength <= 1)
			{
				return true;
			}
			return false;
		}

		private void Disconnect()
		{
			instance.Disconnect();
			stream = null;
			receivedData = null;
			receiveBuffer = null;
			socket = null;
		}
	}

	public class UDP
	{
		public UdpClient socket;

		public IPEndPoint endPoint;

		public UDP()
		{
			endPoint = new IPEndPoint(IPAddress.Parse(instance.ip), instance.port);
		}

		public void Connect(int localPort)
		{
			socket = new UdpClient(localPort);
			socket.Connect(endPoint);
			socket.BeginReceive(ReceiveCallback, null);
			using Packet packet = new Packet();
			SendData(packet);
		}

		public void SendData(Packet packet)
		{
			try
			{
				packet.InsertInt(instance.myId);
				if (socket != null)
				{
					socket.BeginSend(packet.ToArray(), packet.Length(), null, null);
				}
			}
			catch (Exception arg)
			{
				Debug.Log($"Error sending data to server via UDP: {arg}");
			}
		}

		private void ReceiveCallback(IAsyncResult result)
		{
			try
			{
				byte[] array = socket.EndReceive(result, ref endPoint);
				socket.BeginReceive(ReceiveCallback, null);
				if (array.Length < 4)
				{
					instance.Disconnect();
					Debug.Log("UDP failed due to packets being split, in Client class");
				}
				else
				{
					HandleData(array);
				}
			}
			catch
			{
				Disconnect();
			}
		}

		private void HandleData(byte[] data)
		{
			packetsReceived++;
			using (Packet packet = new Packet(data))
			{
				int num = packet.ReadInt();
				byteDown += num;
				data = packet.ReadBytes(num);
			}
			ThreadManagerClient.ExecuteOnMainThread(delegate
			{
				using Packet packet2 = new Packet(data);
				int key = packet2.ReadInt();
				packetHandlers[key](packet2);
			});
		}

		private void Disconnect()
		{
			instance.Disconnect();
			endPoint = null;
			socket = null;
		}
	}

	public static LocalClient instance;

	public static int dataBufferSize = 4096;

	public SteamId serverHost;

	public string ip = "127.0.0.1";

	public int port = 26950;

	public int myId;

	public TCP tcp;

	public UDP udp;

	public static bool serverOwner;

	private bool isConnected;

	public static Dictionary<int, PacketHandler> packetHandlers;

	public static int byteDown;

	public static int packetsReceived;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			Debug.Log("Instance already exists, destroying object");
			UnityEngine.Object.Destroy(this);
		}
	}

	private void Start()
	{
		StartProtocols();
	}

	private void StartProtocols()
	{
		tcp = new TCP();
		udp = new UDP();
	}

	public void ConnectToServer(string ip, string username)
	{
		this.ip = ip;
		StartProtocols();
		InitializeClientData();
		isConnected = true;
		tcp.Connect();
	}

	public static void InitializeClientData()
	{
		packetHandlers = new Dictionary<int, PacketHandler>
		{
			{
				1,
				ClientHandle.Welcome
			},
			{
				2,
				ClientHandle.SpawnPlayer
			},
			{
				3,
				ClientHandle.PlayerPosition
			},
			{
				4,
				ClientHandle.PlayerRotation
			},
			{
				8,
				ClientHandle.ReceivePing
			},
			{
				11,
				ClientHandle.ReceiveStatus
			},
			{
				14,
				ClientHandle.Clock
			},
			{
				51,
				ClientHandle.PlayerFinishedLoading
			},
			{
				9,
				ClientHandle.ConnectionEstablished
			},
			{
				12,
				ClientHandle.GameOver
			},
			{
				56,
				ClientHandle.ShipUpdate
			},
			{
				57,
				ClientHandle.DragonUpdate
			},
			{
				5,
				ClientHandle.DisconnectPlayer
			},
			{
				6,
				ClientHandle.KickPlayer
			},
			{
				7,
				ClientHandle.PlayerDied
			},
			{
				53,
				ClientHandle.SpawnGrave
			},
			{
				16,
				ClientHandle.Ready
			},
			{
				13,
				ClientHandle.StartGame
			},
			{
				15,
				ClientHandle.OpenDoor
			},
			{
				18,
				ClientHandle.DropItem
			},
			{
				22,
				ClientHandle.DropResources
			},
			{
				19,
				ClientHandle.PickupItem
			},
			{
				50,
				ClientHandle.SpawnEffect
			},
			{
				20,
				ClientHandle.WeaponInHand
			},
			{
				21,
				ClientHandle.PlayerHitObject
			},
			{
				46,
				ClientHandle.RemoveResource
			},
			{
				43,
				ClientHandle.PlayerHp
			},
			{
				44,
				ClientHandle.RespawnPlayer
			},
			{
				29,
				ClientHandle.PlayerHit
			},
			{
				23,
				ClientHandle.AnimationUpdate
			},
			{
				45,
				ClientHandle.ShootArrowFromPlayer
			},
			{
				24,
				ClientHandle.FinalizeBuild
			},
			{
				25,
				ClientHandle.OpenChest
			},
			{
				26,
				ClientHandle.UpdateChest
			},
			{
				27,
				ClientHandle.PickupInteract
			},
			{
				28,
				ClientHandle.DropItemAtPosition
			},
			{
				36,
				ClientHandle.DropPowerupAtPosition
			},
			{
				30,
				ClientHandle.MobSpawn
			},
			{
				31,
				ClientHandle.MobMove
			},
			{
				32,
				ClientHandle.MobSetDestination
			},
			{
				55,
				ClientHandle.MobSetTarget
			},
			{
				33,
				ClientHandle.MobAttack
			},
			{
				47,
				ClientHandle.MobSpawnProjectile
			},
			{
				34,
				ClientHandle.PlayerDamageMob
			},
			{
				49,
				ClientHandle.KnockbackMob
			},
			{
				54,
				ClientHandle.Interact
			},
			{
				35,
				ClientHandle.ShrineCombatStart
			},
			{
				52,
				ClientHandle.RevivePlayer
			},
			{
				38,
				ClientHandle.MobZoneToggle
			},
			{
				37,
				ClientHandle.MobZoneSpawn
			},
			{
				39,
				ClientHandle.PickupSpawnZone
			},
			{
				40,
				ClientHandle.ReceiveChatMessage
			},
			{
				41,
				ClientHandle.ReceivePlayerPing
			},
			{
				42,
				ClientHandle.ReceivePlayerArmor
			},
			{
				48,
				ClientHandle.NewDay
			},
			{
				58,
				ClientHandle.ReceiveStats
			}
		};
		Debug.Log("Initializing packets.");
	}

	private void OnApplicationQuit()
	{
		Disconnect();
	}

	public void Disconnect()
	{
		if (isConnected)
		{
			ClientSend.PlayerDisconnect();
			isConnected = false;
			tcp.socket.Close();
			udp.socket.Close();
			Debug.Log("Disconnected from server.");
		}
	}
}

﻿using System;
using WowPacketParser.Enums;
using WowPacketParser.Misc;
using WowPacketParser.Parsing;
using WowPacketParser.Store;
using WowPacketParser.Store.Objects;
using CoreParsers = WowPacketParser.Parsing.Parsers;

namespace WowPacketParserModule.V5_4_0_17359.Parsers
{
    public static class MiscellaneousHandler
    {
        [Parser(Opcode.CMSG_UNKNOWN_903)]
        public static void HandleUnknow903(Packet packet)
        {
            var len = packet.ReadBits(9);
            packet.ReadWoWString("File name", len);
        }

        [HasSniffData]
        [Parser(Opcode.CMSG_LOAD_SCREEN)]
        public static void HandleClientEnterWorld(Packet packet)
        {
            var mapId = packet.ReadEntryWithName<UInt32>(StoreNameType.Map, "Map");
            packet.ReadBit("Loading");
        }

        [Parser(Opcode.CMSG_SET_SELECTION)]
        public static void HandleSetSelection(Packet packet)
        {
            var guid = packet.StartBitStream(1, 3, 4, 6, 0, 5, 7, 2);
            packet.ParseBitStream(guid, 7, 6, 0, 2, 3, 1, 4, 5);
            packet.WriteGuid("Guid", guid);
        }

        [Parser(Opcode.CMSG_REALM_SPLIT)]
        public static void HandleClientRealmSplit(Packet packet)
        {
            packet.ReadEnum<ClientSplitState>("Client State", TypeCode.Int32);
        }

        [Parser(Opcode.CMSG_VIOLENCE_LEVEL)]
        public static void HandleSetViolenceLevel(Packet packet)
        {
            packet.ReadByte("Level");
        }

        [Parser(Opcode.SMSG_SEND_SERVER_LOCATION)]
        public static void HandleSendServerLocation(Packet packet)
        {
            var len1 = packet.ReadBits(7);
            var len2 = packet.ReadBits(7);
            packet.ReadWoWString("Server Location", len1);
            packet.ReadWoWString("Server Location", len2);
        }

        [HasSniffData]
        [Parser(Opcode.SMSG_NEW_WORLD)]
        public static void HandleNewWorld(Packet packet)
        {
            var pos = new Vector4();
            pos.Y = packet.ReadSingle();
            pos.O = packet.ReadSingle();
            pos.Z = packet.ReadSingle();
            pos.X = packet.ReadSingle();
            CoreParsers.MovementHandler.CurrentMapId = (uint)packet.ReadEntryWithName<Int32>(StoreNameType.Map, "Map");
            packet.WriteLine("Position: {0}", pos);

            packet.AddSniffData(StoreNameType.Map, (int)CoreParsers.MovementHandler.CurrentMapId, "NEW_WORLD");
        }

        [Parser(Opcode.SMSG_REALM_SPLIT)]
        public static void HandleServerRealmSplit(Packet packet)
        {
            var len = packet.ReadBits(7);
            packet.ReadEnum<PendingSplitState>("Split State", TypeCode.Int32);
            packet.ReadEnum<ClientSplitState>("Client State", TypeCode.Int32);
            packet.ReadWoWString("Split Date", len);
        }

        [Parser(Opcode.CMSG_AREATRIGGER)]
        public static void HandleClientAreaTrigger(Packet packet)
        {
            packet.ReadInt32("Area Trigger Id");
            packet.ReadBit("Unk bit1");
            packet.ReadBit("Unk bit2");
        }

        [Parser(Opcode.SMSG_WEATHER)]
        public static void HandleWeatherStatus(Packet packet)
        {
            packet.ReadBit("Unk bit");
            packet.ReadSingle("Grade");
            packet.ReadEnum<WeatherState>("State", TypeCode.Int32);
        }

        [Parser(Opcode.SMSG_HOTFIX_INFO)]
        public static void HandleHotfixInfo(Packet packet)
        {
            var count = packet.ReadBits("Count", 20);

            for (var i = 0; i < count; ++i)
            {
                packet.ReadInt32("Hotfixed entry", i);
                packet.ReadTime("Hotfix date", i);
                packet.ReadEnum<DB2Hash>("Hotfix DB2 File", TypeCode.Int32, i);
            }
        }

        [Parser(Opcode.SMSG_PLAY_SOUND)]
        public static void HandlePlaySound(Packet packet)
        {
            var guid = new byte[8];

            packet.StartBitStream(guid, 6, 7, 5, 2, 1, 4, 0, 3);
            packet.ParseBitStream(guid, 7, 0, 5, 4, 3, 1, 2, 6);

            var sound = packet.ReadUInt32("Sound Id");
            packet.WriteGuid("Guid", guid);

            Storage.Sounds.Add(sound, packet.TimeSpan);
        }

        [Parser(Opcode.SMSG_WORLD_SERVER_INFO)]
        public static void HandleWorldServerInfo(Packet packet)
        {
            packet.ReadInt32("Instance Difficulty ID");
            packet.ReadTime("Last Weekly Reset");
            packet.ReadByte("Byte18");

            var bit20 = packet.ReadBit();
            var bit38 = packet.ReadBit();
            var bit2C = packet.ReadBit();
            var bit14 = packet.ReadBit();

            if (bit20)
                packet.ReadInt32("Int1C");
            if (bit38)
                packet.ReadInt32("Int34");
            if (bit2C)
                packet.ReadInt32("Int28");
            if (bit14)
                packet.ReadInt32("Int10");
        }

        [Parser(Opcode.SMSG_FEATURE_SYSTEM_STATUS)]
        public static void HandleFeatureSystemStatus(Packet packet)
        {
            packet.ReadInt32("Scroll of Resurrections Remaining");
            packet.ReadInt32("Realm Id?");
            packet.ReadByte("Complain System Status");
            packet.ReadInt32("Unused Int32");
            packet.ReadInt32("Scroll of Resurrections Per Day");

            packet.ReadBit("bit26");
            packet.ReadBit("bit54");
            packet.ReadBit("bit30");
            var sessionTimeAlert = packet.ReadBit("Session Time Alert");
            packet.ReadBit("bit38");
            var quickTicket = packet.ReadBit("EuropaTicketSystemEnabled");
            packet.ReadBit("bit25");
            packet.ReadBit("bit24");
            packet.ReadBit("bit28");

            if (sessionTimeAlert)
            {
                packet.ReadInt32("Int10");
                packet.ReadInt32("Int18");
                packet.ReadInt32("Int14");
            }

            if (quickTicket)
            {
                packet.ReadInt32("Unk5");
                packet.ReadInt32("Unk6");
                packet.ReadInt32("Unk7");
                packet.ReadInt32("Unk8");
            }
        }

        [Parser(Opcode.SMSG_REQUEST_CEMETERY_LIST_RESPONSE)]
        public static void HandleRequestCemeteryListResponse(Packet packet)
        {
            packet.ReadBit("Is MicroDungeon"); // Named in WorldMapFrame.lua
            var count = packet.ReadBits("Count", 22);

            for (var i = 0; i < count; ++i)
                packet.ReadInt32("Cemetery Id", i);
        }

        [Parser(Opcode.SMSG_UNKNOWN_274)]
        public static void HandleUnknow274(Packet packet)
        {
            byte[][] guid1;
            byte[][] guid2;

            var counter = packet.ReadBits(19);
            
            guid1 = new byte[counter][];
            guid2 = new byte[counter][];
            for (var i = 0; i < counter; ++i)
            {
                guid1[i] = new byte[8];
                guid2[i] = new byte[8];
                
                packet.StartBitStream(guid2[i], 6, 3);
                packet.StartBitStream(guid1[i], 6, 1, 2, 3);
                packet.ReadBits(4);
                packet.StartBitStream(guid1[i], 5, 0, 7);
                packet.StartBitStream(guid2[i], 0, 7, 5);
                guid1[i][4] = packet.ReadBit();
                packet.StartBitStream(guid2[i], 1, 4, 2);
            }
            
            for (var i = 0; i < counter; ++i)
            {
                packet.ReadXORByte(guid1[i], 1);
                packet.ReadInt32("Int3Ch", i);
                packet.ReadXORByte(guid1[i], 2);
                packet.ReadXORByte(guid2[i], 6);
                packet.ReadXORByte(guid1[i], 4);
                packet.ReadXORByte(guid2[i], 7);
                packet.ReadXORByte(guid1[i], 6);
                packet.ReadXORByte(guid1[i], 7);

                packet.ReadInt32("Int40h", i);

                packet.ReadXORByte(guid2[i], 5);
                packet.ReadXORByte(guid1[i], 5);
                packet.ReadXORByte(guid2[i], 2);

                packet.ReadPackedTime("Time", i);
                packet.ReadInt32("Int14h", i);

                packet.ReadXORByte(guid2[i], 0);
                packet.ReadXORByte(guid1[i], 3);
                packet.ReadXORByte(guid2[i], 1);
                packet.ReadXORByte(guid1[i], 0);
                packet.ReadXORByte(guid2[i], 4);
                packet.ReadXORByte(guid2[i], 3);

                packet.WriteGuid("Guid1", guid1[i], i);
                packet.WriteGuid("Guid2", guid2[i], i);
            }
        }

        [Parser(Opcode.SMSG_UNKNOWN_1024)]
        public static void HandleUnknown1024(Packet packet)
        {
            var guid = new byte[8];

            packet.StartBitStream(guid, 7, 4, 1, 5, 0, 3, 6);
            packet.ReadBits("bits0", 3);
            guid[2] = packet.ReadBit();
            packet.ParseBitStream(guid, 3, 6, 0, 5, 1, 7, 4, 2);

            packet.WriteGuid("GUID", guid);
        }

        [Parser(Opcode.SMSG_UNKNOWN_6775)]
        public static void HandleUnknown6775(Packet packet)
        {
            var guid = new byte[8];

            packet.StartBitStream(guid, 6, 5, 7, 2, 4, 0, 1, 3);
            packet.ParseBitStream(guid, 1, 5, 0, 6, 4, 2, 3, 7);

            packet.WriteGuid("Guid", guid);
        }

        [Parser(Opcode.SMSG_UNKNOWN_3)]
        public static void HandleUnknown3(Packet packet)
        {
            var guid = new byte[8];
            packet.StartBitStream(guid, 1, 3, 4, 0, 5, 6, 2, 7);
            packet.ParseBitStream(guid, 1, 4, 0, 6, 3, 7, 5, 2);

            packet.WriteGuid("Guid", guid);
        }

        [Parser(Opcode.SMSG_UNKNOWN_1457)]
        public static void HandleUnknown1457(Packet packet)
        {
            var guid = new byte[8];

            packet.StartBitStream(guid, 5, 2, 3, 6, 7, 1, 0, 4);
            packet.ParseBitStream(guid, 1, 3);
            packet.ReadInt32("int18h");
            packet.ParseBitStream(guid, 7, 0, 5, 4, 2, 6);

            packet.WriteGuid("GUID", guid);
        }

        [Parser(Opcode.SMSG_UNKNOWN_5738)]
        public static void HandleUnknown5738(Packet packet)
        {
            var guid = new byte[8];

            packet.StartBitStream(guid, 3, 0, 5, 4, 2, 7, 1, 6);
            packet.ParseBitStream(guid, 2, 6, 3);
            packet.ReadSingle("Speed");
            packet.ParseBitStream(guid, 0, 4, 1, 5, 7);

            packet.WriteGuid("GUID", guid);
        }

        [Parser(Opcode.SMSG_UNKNOWN_56)]
        public static void HandleUnknown56(Packet packet)
        {
            packet.ReadInt32("Int14");
            packet.ReadInt32("Int10");
        }

        [Parser(Opcode.SMSG_UNKNOWN_5125)]
        public static void HandleUnknown5125(Packet packet)
        {
            var guid = new byte[8];

            packet.StartBitStream(guid, 2, 7, 0, 6, 5, 3, 1, 4);
            packet.ParseBitStream(guid, 2, 0, 1, 7, 4, 5);
            packet.ReadInt32("Int18");
            packet.ParseBitStream(guid, 3, 6);

            packet.WriteGuid("GUID", guid);
        }

        [Parser(Opcode.SMSG_UNKNOWN_57)]
        public static void HandleUnknown57(Packet packet)
        {
            packet.ReadByte("Byte20");
            packet.ReadInt32("Int1C");
            packet.ReadEntryWithName<UInt32>(StoreNameType.Spell, "Spell ID");

            var bit10 = !packet.ReadBit();
            var bit14 = !packet.ReadBit();

            if (bit10)
                packet.ReadInt32("Int10");
            if (bit14)
                packet.ReadInt32("Int14");
        }

        [Parser(Opcode.SMSG_UNKNOWN_170)]
        public static void HandleUnknown170(Packet packet)
        {
            packet.ReadInt32("Int14");
            packet.ReadBits("bits10", 2);
        }

        [Parser(Opcode.SMSG_UNKNOWN_5963)]
        public static void HandleUnknown5963(Packet packet)
        {
            var guid = new byte[8];

            packet.StartBitStream(guid, 1, 7, 4, 5, 2, 6, 0, 3);
            packet.ParseBitStream(guid, 1, 6, 5, 0);
            packet.ReadSingle("Speed");
            packet.ParseBitStream(guid, 4, 3, 2, 7);

            packet.WriteGuid("GUID", guid);
        }

        [Parser(Opcode.SMSG_UNKNOWN_5730)]
        public static void HandleUnknown5730(Packet packet)
        {
            var guid = new byte[8];

            packet.StartBitStream(guid, 6, 3, 4, 2, 5, 7, 0, 1);
            packet.ReadXORByte(guid, 4);
            packet.ReadSingle("Speed");
            packet.ParseBitStream(guid, 7, 1, 6, 3, 0, 2, 5);

            packet.WriteGuid("GUID", guid);
        }

        [Parser(Opcode.SMSG_UNKNOWN_316)]
        public static void HandleUnknown316(Packet packet)
        {
            var guid = new byte[8];

            packet.ReadInt32("Int10"); // sub_7B4A0D
            var len = packet.ReadInt32("Int78");

            packet.ReadBytes(len);

            guid[1] = packet.ReadBit();
            packet.ReadBit("bit20");
            packet.StartBitStream(guid, 7, 2, 6, 3, 4, 5, 0);

            packet.ReadBit("bit21");
            packet.ParseBitStream(guid, 6, 1, 4, 2, 5, 0, 3, 7);

            packet.WriteGuid("Guid3", guid);
        }

        [Parser(Opcode.SMSG_UNKNOWN_2048)]
        public static void HandleUnknown2048(Packet packet)
        {
            var guid1 = new byte[8];
            var guid2 = new byte[8];

            guid1[7] = packet.ReadBit();
            packet.StartBitStream(guid2, 6, 5);
            packet.StartBitStream(guid1, 3, 0, 6);
            guid2[2] = packet.ReadBit();
            var bit18 = packet.ReadBit();
            guid1[5] = packet.ReadBit();
            guid2[1] = packet.ReadBit();
            guid2[0] = packet.ReadBit();
            packet.StartBitStream(guid1, 1, 4);
            packet.StartBitStream(guid2, 4, 7, 3);
            guid1[2] = packet.ReadBit();
            packet.ReadXORByte(guid1, 4);
            packet.ReadXORByte(guid2, 0);
            packet.ReadSingle("Float2C");
            packet.ReadXORByte(guid2, 6);
            packet.ReadSingle("Float30");
            packet.ReadSingle("Float1C");
            packet.ReadXORByte(guid1, 0);
            packet.ReadInt32("Int24");
            packet.ReadXORByte(guid1, 3);
            packet.ReadXORByte(guid2, 3);
            packet.ReadInt16("Int20");
            packet.ReadXORByte(guid1, 7);
            packet.ReadInt16("Int40");
            packet.ReadXORByte(guid2, 5);
            packet.ReadXORByte(guid2, 2);
            packet.ReadSingle("Float28");
            packet.ReadXORByte(guid2, 4);
            packet.ReadXORByte(guid2, 1);
            packet.ReadXORByte(guid1, 2);
            packet.ReadXORByte(guid2, 7);
            packet.ReadXORByte(guid1, 5);
            packet.ReadXORByte(guid1, 6);
            packet.ReadXORByte(guid1, 1);

            packet.WriteGuid("Guid1", guid1);
            packet.WriteGuid("Guid2", guid2);
        }

        [Parser(Opcode.SMSG_UNKNOWN_6418)]
        public static void HandleUnknown6418(Packet packet)
        {
            var guid = new byte[8];

            packet.StartBitStream(guid, 0, 5, 7, 2, 1, 4, 6, 3);
            packet.ReadXORByte(guid, 4);
            packet.ReadXORByte(guid, 3);
            packet.ReadXORByte(guid, 6);
            packet.ReadXORByte(guid, 2);
            packet.ReadXORByte(guid, 0);
            packet.ReadXORByte(guid, 7);
            packet.ReadByte("Byte18");
            packet.ReadXORByte(guid, 1);
            packet.ReadByte("Byte19");
            packet.ReadXORByte(guid, 5);

            packet.WriteGuid("Guid2", guid);
        }

        [Parser(Opcode.SMSG_UNKNOWN_443)]
        public static void HandleUnknown443(Packet packet)
        {
            var guid1 = new byte[8];
            var guid2 = new byte[8];

            packet.StartBitStream(guid1, 6, 2);
            packet.StartBitStream(guid2, 5, 6, 7, 4, 3, 2, 1);
            packet.StartBitStream(guid1, 1, 5, 7, 4, 3);
            guid2[0] = packet.ReadBit();
            guid1[0] = packet.ReadBit();
            packet.ReadXORByte(guid2, 6);
            packet.ReadXORByte(guid2, 1);
            packet.ReadXORByte(guid2, 4);
            packet.ReadXORByte(guid2, 2);
            packet.ReadXORByte(guid1, 5);
            packet.ReadXORByte(guid1, 0);
            packet.ReadXORByte(guid1, 2);
            packet.ReadXORByte(guid2, 3);
            packet.ReadXORByte(guid2, 0);
            packet.ReadXORByte(guid1, 3);
            packet.ReadXORByte(guid1, 7);
            packet.ReadXORByte(guid1, 1);
            packet.ReadXORByte(guid2, 7);
            packet.ReadXORByte(guid1, 4);
            packet.ReadXORByte(guid1, 6);
            packet.ReadXORByte(guid2, 5);

            packet.WriteGuid("Guid1", guid1);
            packet.WriteGuid("Guid2", guid2);
        }

        [Parser(Opcode.SMSG_UNKNOWN_67)]
        public static void HandleUnknown67(Packet packet)
        {
            packet.ReadInt32("Count?");
        }

        [Parser(Opcode.SMSG_UNKNOWN_442)]
        public static void HandleUnknown442(Packet packet)
        {
            var guid = new byte[8];

            packet.StartBitStream(guid, 5, 6, 7, 0, 4, 3, 1, 2);
            packet.ReadXORByte(guid, 4);
            packet.ReadXORByte(guid, 6);
            packet.ReadByte("Byte1D");
            packet.ReadByte("Byte1C");
            packet.ReadXORByte(guid, 0);
            packet.ReadXORByte(guid, 7);
            packet.ReadXORByte(guid, 2);
            packet.ReadXORByte(guid, 1);
            packet.ReadXORByte(guid, 3);
            packet.ReadInt32("Int18");
            packet.ReadXORByte(guid, 5);

            packet.WriteGuid("Guid", guid);
        }

        [Parser(Opcode.SMSG_UNKNOWN_2093)]
        public static void HandleUnknown2093(Packet packet)
        {
            var guid = new byte[8];

            packet.StartBitStream(guid, 2, 6, 0, 7, 1, 3, 4, 5);
            packet.ReadXORByte(guid, 0);
            packet.ReadXORByte(guid, 6);
            packet.ReadSingle("Speed");
            packet.ReadXORByte(guid, 5);
            packet.ReadXORByte(guid, 7);
            packet.ReadXORByte(guid, 3);
            packet.ReadXORByte(guid, 4);
            packet.ReadXORByte(guid, 1);
            packet.ReadXORByte(guid, 2);

            packet.WriteGuid("Guid", guid);
        }

        [Parser(Opcode.SMSG_UNKNOWN_4501)]
        public static void HandleUnknown4501(Packet packet)
        {
            var guid1 = new byte[8];
            var guid2 = new byte[8];
            var guid3 = new byte[8];

            var bit20 = false;
            var bit24 = false;

            var bit28 = packet.ReadBit();
            if (bit28)
            {
                guid1[0] = packet.ReadBit();
                guid1[1] = packet.ReadBit();
                guid1[6] = packet.ReadBit();
                guid1[4] = packet.ReadBit();
                bit24 = !packet.ReadBit();
                guid1[3] = packet.ReadBit();
                bit20 = !packet.ReadBit();

                packet.ReadBit(); // fake bit

                guid2[2] = packet.ReadBit();
                guid2[3] = packet.ReadBit();
                guid2[4] = packet.ReadBit();
                guid2[6] = packet.ReadBit();
                guid2[7] = packet.ReadBit();
                guid2[5] = packet.ReadBit();
                guid2[1] = packet.ReadBit();
                guid2[0] = packet.ReadBit();
                guid1[7] = packet.ReadBit();
                guid1[5] = packet.ReadBit();
                guid1[2] = packet.ReadBit();
            }

            var bit38 = packet.ReadBit();
            guid3[6] = packet.ReadBit();
            guid3[0] = packet.ReadBit();
            guid3[4] = packet.ReadBit();
            guid3[1] = packet.ReadBit();
            guid3[2] = packet.ReadBit();
            guid3[3] = packet.ReadBit();
            guid3[5] = packet.ReadBit();
            guid3[7] = packet.ReadBit();

            if (bit38)
            {
                packet.ReadInt32("Int30");
                packet.ReadInt32("Int34");
            }

            if (bit28)
            {
                packet.ReadXORByte(guid1, 0);

                if (bit20)
                    packet.ReadInt32("Int20");

                packet.ReadXORByte(guid1, 4);
                packet.ReadXORByte(guid1, 5);
                packet.ReadXORByte(guid2, 7);
                packet.ReadXORByte(guid2, 6);
                packet.ReadXORByte(guid2, 1);
                packet.ReadXORByte(guid2, 4);
                packet.ReadXORByte(guid2, 0);
                packet.ReadXORByte(guid2, 2);
                packet.ReadXORByte(guid2, 5);
                packet.ReadXORByte(guid2, 3);
                packet.ReadXORByte(guid1, 3);

                if (bit24)
                    packet.ReadByte("Byte24");

                packet.ReadXORByte(guid1, 1);
                packet.ReadXORByte(guid1, 7);
                packet.ReadXORByte(guid1, 2);
                packet.ReadXORByte(guid1, 6);

                packet.WriteGuid("Guid2", guid1);
                packet.WriteGuid("Guid3", guid2);
            }

            packet.ReadXORByte(guid3, 5);
            packet.ReadXORByte(guid3, 6);
            packet.ReadXORByte(guid3, 4);
            packet.ReadInt32("Int48"); // spellId?
            packet.ReadXORByte(guid3, 7);
            packet.ReadXORByte(guid3, 1);
            packet.ReadXORByte(guid3, 3);
            packet.ReadInt32("Int4C"); // duration?
            packet.ReadXORByte(guid3, 2);
            packet.ReadXORByte(guid3, 0);

            packet.WriteGuid("Guid8", guid3);
        }

        [Parser(Opcode.SMSG_UNKNOWN_1194)]
        public static void HandleUnknown1194(Packet packet)
        {
            var guid = new byte[8];

            packet.StartBitStream(guid, 0, 3, 5, 4, 6, 7, 1, 2);
            packet.ParseBitStream(guid, 4, 6, 7, 0, 2, 5, 1, 3);

            packet.WriteGuid("Guid", guid);
        }
    }
}

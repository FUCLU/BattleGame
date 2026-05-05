using BattleGame.Server.Database;
using BattleGame.Server.Services;
using Xunit;

namespace BattleGame.Test.Server
{
    public class MatchmakingServiceTests
    {
        private static MatchmakingService CreateService()
        {
            // MatchRepository is only used when ending a match.
            // These tests do not call EndMatch, so a dummy connection string is sufficient.
            var repo = new MatchRepository("Host=localhost;Port=1;Username=x;Password=x;Database=x");
            return new MatchmakingService(repo);
        }

        [Fact]
        public void CreateRoom_OwnerCanSeeRoomWithZeroPlayers()
        {
            var sut = CreateService();
            const int ownerId = 10;

            var (roomId, success) = sut.CreateRoom("room-a", "pw", ownerId, "owner", null!);

            Assert.True(success);
            var room = Assert.Single(sut.GetRooms(ownerId));
            Assert.Equal(roomId, room.RoomId);
            Assert.Equal(0, room.CurrentPlayers);
            Assert.True(room.IsOwner);
        }

        [Fact]
        public void JoinAndLeave_FullOwnerFlowIsConsistent()
        {
            var sut = CreateService();
            const int ownerId = 10;
            const int guestId = 20;

            var (roomId, _) = sut.CreateRoom("room-a", "pw", ownerId, "owner", null!);

            var guestViewBeforeOwnerJoin = Assert.Single(sut.GetRooms(guestId));
            Assert.Equal(roomId, guestViewBeforeOwnerJoin.RoomId);
            Assert.Equal(0, guestViewBeforeOwnerJoin.CurrentPlayers);
            Assert.False(guestViewBeforeOwnerJoin.IsOwner);

            bool ownerJoined = sut.JoinRoom(roomId, "pw", ownerId, "owner", null!, out _);
            Assert.True(ownerJoined);

            var ownerRoomAfterJoin = Assert.Single(sut.GetRooms(ownerId));
            Assert.Equal(1, ownerRoomAfterJoin.CurrentPlayers);
            Assert.True(ownerRoomAfterJoin.IsOwner);

            var guestRoomAfterJoin = Assert.Single(sut.GetRooms(guestId));
            Assert.Equal(roomId, guestRoomAfterJoin.RoomId);
            Assert.Equal(1, guestRoomAfterJoin.CurrentPlayers);
            Assert.False(guestRoomAfterJoin.IsOwner);

            sut.LeaveRoom(roomId, ownerId, null!);

            var ownerRoomAfterLeave = Assert.Single(sut.GetRooms(ownerId));
            Assert.Equal(0, ownerRoomAfterLeave.CurrentPlayers);
            Assert.True(ownerRoomAfterLeave.IsOwner);

            var guestRoomAfterOwnerLeave = Assert.Single(sut.GetRooms(guestId));
            Assert.Equal(roomId, guestRoomAfterOwnerLeave.RoomId);
            Assert.Equal(0, guestRoomAfterOwnerLeave.CurrentPlayers);
            Assert.False(guestRoomAfterOwnerLeave.IsOwner);
        }
    }
}

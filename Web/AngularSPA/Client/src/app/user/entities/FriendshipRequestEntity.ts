export class FriendshipRequestEntity {
    id: string;
    requester: string;
    requestee: string;
    createdAt: Date;
    modifiedAt: Date;
    outcome: string;
}
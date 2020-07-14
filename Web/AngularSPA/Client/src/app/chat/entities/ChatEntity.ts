export class ChatEntity {
    id: string;
    name: string;
    owner: string;
    members: string[];
    capacity: number;

    isChatroom() {
        return this.capacity > 2;
    }
}
import { User, userSchema, userListSchema } from "src/app/user/models/User";
import { schema } from "normalizr";

export const chatSchema = new schema.Entity('chats', {
    owner: userSchema,
    members: userListSchema
});

export const chatListSchema = new schema.Array(chatSchema);

export interface Chat {
    id: string,
    name: string,
    capacity: number,
    owner: User,
    members: User[]
}
import { User, userSchema } from "./User";
import { schema } from "normalizr";

export const friendshipSchema = new schema.Object({
    user1: userSchema,
    user2: userSchema
});

export const friendshipListSchema = new schema.Array(friendshipSchema);

export class Friendship {
    id: string;
    user1: User;
    user2: User;
    createdAt: Date;
    modifiedAt: Date;
}
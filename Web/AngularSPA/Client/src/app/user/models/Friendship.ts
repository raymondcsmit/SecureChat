import { User, userSchema } from "./User";
import { schema } from "normalizr";

export const friendshipSchema = new schema.Entity('friendships', {
    user1: userSchema,
    user2: userSchema
});

export const friendshipListSchema = new schema.Array(friendshipSchema);

export interface Friendship {
    id: string;
    user1: User;
    user2: User;
    createdAt: Date;
    modifiedAt: Date;
}
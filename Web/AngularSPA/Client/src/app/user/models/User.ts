import { Profile } from "./Profile";
import { schema } from 'normalizr';

export const userSchema = new schema.Entity('users', {});

export const userListSchema = new schema.Array(userSchema);

export interface User {
    id: string;
    userName: string;
    email?: string;
    profile: Profile;
}
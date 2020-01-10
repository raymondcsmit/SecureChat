import { Profile } from "./Profile";

export class User {
    id: string;
    userName: string;
    email?: string;
    profile: Profile;
}
import { Profile } from "../models/Profile";
import { User } from "../models/User";

export class UserEntity implements User {
    id: string;
    userName: string;
    email: string;
    profile: Profile;
}
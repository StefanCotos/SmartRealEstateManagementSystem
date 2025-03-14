import { Estate } from "./estate.model";

export interface Image {
    id?: string;
    estateId: Estate;
    extension: string;
}
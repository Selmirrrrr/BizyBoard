export interface Credentials {
    email: string;
    password: string;
}

export interface Email {
    email: string;
}

export interface Dossier {
    number: number;
    name: string;
    exercices: Exercice[]
}

export interface Exercice {
    year: number;
    start: Date;
    end: Date;
    description: string;
    isClosed: boolean;
    dossier: number;
}
import { ImmutableObject } from "seamless-immutable";

export interface Config {
  layerName: string;
  cityField: string;
  TabelCity: string;
}

export type IMConfig = ImmutableObject<Config>;

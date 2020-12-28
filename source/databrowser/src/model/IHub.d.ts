import {IExtraValue} from "./IExtraValue";

export interface IHub {
  name: string;
  slogan: string;
  supportedLanguages: string[];
  defaultLanguage: string;
  maxObservationsAfterCriteria: number;
  logoURL: string;
  backgroundMediaURL: string;
  description: string;
  extras: IExtraValue[];
}

import {IExtraValue} from "./IExtraValue";

export interface ICategory {
  label: string;
  childrenCategories: Array<ICategory>;
  extras: Array<IExtraValue>;
  datasetIdentifiers: Array<string>;
  image: string;
}
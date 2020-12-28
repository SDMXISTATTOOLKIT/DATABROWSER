import {IExtraValue} from "./IExtraValue";
import {ICategory} from "./ICategory";

export interface ICategoryGroup {
  label: string;
  extras: Array<IExtraValue>;
  categories: Array<ICategory>;
}
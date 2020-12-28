import {IFilter} from "./IFilter";
import {IDataset} from "./IDataset";
import {ICategory} from "./ICategory";
import {ICategoryGroup} from "./ICategoryGroup";

export interface ICategoryProvider {
  categoryGroups: Array<ICategoryGroup>;
  datasets: Map<String, IDataset>;
  searchCategories(filter?: IFilter): Array<ICategory>;
  searchDataset(filter?: IFilter): Array<IDataset>;
}
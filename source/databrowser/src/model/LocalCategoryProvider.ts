import {ICategoryProvider} from "./ICategoryProvider";
import {IFilter} from "./IFilter";
import {ICategoryGroup} from "./ICategoryGroup";
import {ICategory} from "./ICategory";
import {IDataset} from "./IDataset";

export class LocalCategoryProvider implements ICategoryProvider {

  categoryGroups: Array<ICategoryGroup>;

  datasets: Map<String, IDataset>;

  uncategorizedDatasets: IDataset[];

  nodeId: number;

  constructor(categories: Array<ICategoryGroup>, datasets: Map<String, IDataset>, uncategorizedDatasets: IDataset[] = [], nodeId: number = -1) {
    this.datasets = datasets || {};
    this.categoryGroups = categories || [];
    this.uncategorizedDatasets = uncategorizedDatasets || [];
    this.nodeId = nodeId || -1;
  }

  searchCategories(filter?: IFilter): Array<ICategory> {

    let result: Array<ICategory> = [];

    let searchText: string;
    if (filter) {
      searchText = filter.text.toLowerCase();
    }

    this.categoryGroups.forEach(categ => {

      if (!searchText) {
        result.push(...categ.categories);
      } else {
        categ.categories.forEach(category => {
          let matchFound: boolean = category.label.toLowerCase().indexOf(searchText) >= 0;

          if (matchFound) {
            result.push(category);
          }
        });
      }
    });

    return result;
  }

  searchDataset(filter?: IFilter): IDataset[] {
    return [];
  }
}
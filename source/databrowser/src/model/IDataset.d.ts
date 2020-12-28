import {IDatasetDistribution} from "./IDatasetDistribution";
import {IExtraValue} from "./IExtraValue";
import {IDatasetAttachment} from './IDatasetAttachment';

export interface IDataset {
  identifier: string;
  title: string;
  description: string;
  source: string;
  distributions: Array<IDatasetDistribution>;
  extras: Array<IExtraValue>;
  keywords: string[]
  catalogType: string
  attachedDataFiles: IDatasetAttachment[]
}
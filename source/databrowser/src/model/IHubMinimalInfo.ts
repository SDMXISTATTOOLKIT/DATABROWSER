import {IHub} from "./IHub";
import {IHubMinimalNode} from "./IHubMinimalNode";

export interface IHubMinimalInfo {
  hub: IHub,
  nodes: IHubMinimalNode[]
}
import {IExtraValue} from "./IExtraValue";
import {IHubMinimalNode} from "./IHubMinimalNode";

export interface IHubNode extends IHubMinimalNode {
  nodeType: string;
  logoURL: string;
  slogan: string;
  description: string;
  extras: IExtraValue[];
}
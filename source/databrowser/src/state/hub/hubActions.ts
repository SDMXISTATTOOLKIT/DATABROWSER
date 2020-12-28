import {initRequest} from "../../middlewares/request/requestActions";
import {getHubMinimalInfoUrl} from "../../serverApi/urls";

export const HUB_FETCH = "hub/fetch";
export const HUB_CLEAR = "hub/clear";

export const fetchHub = () => initRequest(
  HUB_FETCH,
  getHubMinimalInfoUrl(),
  undefined,
  undefined,
  t => ({
    onStart: t("domains.hub.messages.fetch.start")
  })
);

export const clearHub = () => ({
  type: HUB_CLEAR
})
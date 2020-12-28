import {Reducer} from "redux";
import {REQUEST_SUCCESS} from "../../middlewares/request/requestActions";
import {
    NODE_PERMISSIONS_CONFIG_CLEAR,
    NODE_PERMISSIONS_CONFIG_FETCH,
    NODE_PERMISSIONS_CONFIG_SEND
} from './nodePermissionsConfigActions';

export type NodePermissionsConfigState = {
    permissions: any[] | null
};

const nodePermissionsConfigReducer: Reducer<NodePermissionsConfigState> = (
    state = {
        permissions: null
    },
    action
) => {
    switch (action.type) {
        case NODE_PERMISSIONS_CONFIG_CLEAR: {
            return {
                ...state,
                permissions: null
            };
        }
        case REQUEST_SUCCESS: {
            switch (action.payload.label) {
                case NODE_PERMISSIONS_CONFIG_FETCH: {
                    return {
                        state,
                        permissions: action.payload.response
                    };
                }
                case NODE_PERMISSIONS_CONFIG_SEND:
                    return {permissions: null};
                default:
                    return state;
            }
        }
        default:
            return state;
    }
};

export default nodePermissionsConfigReducer;
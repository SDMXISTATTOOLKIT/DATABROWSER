import {Reducer} from 'redux';
import {REQUEST_ERROR, RequestMethod} from '../../middlewares/request/requestActions';
import {DELETE_CONFLICT_MODAL_CLOSE} from './deleteConflictModalActions';

type DeleteConflictModalState = {
    response: any | null,
    onForce: any | null
};

const deleteConflictModalReducer: Reducer<DeleteConflictModalState> = (
    state = {
        response: null,
        onForce: null
    },
    action
) => {
    switch (action.type) {
        case REQUEST_ERROR:
            if (action.payload.statusCode === 409 && action.payload.method === RequestMethod.DELETE) {
                return {
                    ...state,
                    response: action.payload.response,
                    onForce: action.payload.extra?.onDeleteConflictForce || null
                };
            } else {
                return state;
            }
        case DELETE_CONFLICT_MODAL_CLOSE:
            return {
                ...state,
                response: null,
                onForce: null
            };
        default:
            return state;
    }
};

export default deleteConflictModalReducer;
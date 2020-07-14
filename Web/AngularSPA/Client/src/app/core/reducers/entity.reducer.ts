import { EntityActionsUnion, EntityActionTypes } from "../actions/entity.actions";
import { EntityAdapter, EntityState } from "@ngrx/entity";
import { state } from "@angular/animations";

export function entityReducer<T, U extends EntityState<T>>(action: EntityActionsUnion<T>, entityAdapter: EntityAdapter<T>, entityState: U): U {
    switch (action.type) {
        case EntityActionTypes.AddEntity: {
            if (action.payload.entity == null) {
                return entityState;
            }
            return entityAdapter.addOne(action.payload.entity, entityState);
        }

        case EntityActionTypes.AddEntities: {
            return entityAdapter.addMany(action.payload.entities, entityState);
        }

        case EntityActionTypes.LoadEntities: {
            return entityAdapter.addAll(action.payload.entities, entityState);
        }

        case EntityActionTypes.UpsertEntity: {
            if (action.payload.entity == null) {
                return entityState;
            }
            return entityAdapter.upsertOne(action.payload.entity, entityState);
        }

        case EntityActionTypes.UpsertEntities: {
            return entityAdapter.upsertMany(action.payload.entities, entityState);
        }

        case EntityActionTypes.DeleteEntity: {
            if (action.payload.id == null) {
                return entityState;
            }
            return entityAdapter.removeOne(action.payload.id, entityState);
        }

        case EntityActionTypes.DeleteEntities: {
            return entityAdapter.removeMany(action.payload.ids, entityState);
        }

        case EntityActionTypes.DeleteEntitiesByPredicate: {
            return entityAdapter.removeMany(action.payload.predicate, entityState);
        }

        default: {
          return entityState;
        }
      }
}
import { EntityActionsUnion, EntityActionTypes } from "../actions/entity.actions";
import { EntityAdapter, EntityState } from "@ngrx/entity";

export function entityReducer<T, U extends EntityState<T>>(action: EntityActionsUnion<T>, entityAdapter: EntityAdapter<T>, entityState: U): U {
    switch (action.type) {
        case EntityActionTypes.AddEntity: {
            return entityAdapter.addOne(action.payload.entity, entityState);
        }

        case EntityActionTypes.AddEntities: {
            return entityAdapter.addMany(action.payload.entities, entityState);
        }

        case EntityActionTypes.LoadEntities: {
            return entityAdapter.addAll(action.payload.entities, entityState);
        }

        case EntityActionTypes.UpsertEntity: {
            return entityAdapter.upsertOne(action.payload.entity, entityState);
        }

        case EntityActionTypes.UpsertEntities: {
            return entityAdapter.upsertMany(action.payload.entities, entityState);
        }

        case EntityActionTypes.DeleteEntity: {
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